if (!([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Start-Process -Verb runas -FilePath powershell.exe -ArgumentList "$PSCommandPath"
        break
    }
    
####################################
$path_to_use = Get-Location

# Set location without displaying it
Set-Location $path_to_use | Out-Null
####################################
# Set the registry key path
$regKeyPath = "HKCU:\Console"

# Set the registry value name and data
$valueName = "QuickEdit"
$valueData = 0

# Create the registry value
New-ItemProperty -Path $regKeyPath -Name $valueName -Value $valueData -PropertyType DWORD -Force | Out-Null
####################################
####################################
# Hide Console
# Define the function to hide the console
Add-Type -Name Window -Namespace Console -MemberDefinition '
[DllImport("Kernel32.dll")]
public static extern IntPtr GetConsoleWindow();

[DllImport("user32.dll")]
public static extern bool ShowWindow(IntPtr hWnd, Int32 nCmdShow);
'

function Hide-Console {
    $consolePtr = [Console.Window]::GetConsoleWindow()
    [Console.Window]::ShowWindow($consolePtr, 0)
}

# Call the Hide-Console function
Hide-Console
####################################

####################################
Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

# Create form
$form = New-Object System.Windows.Forms.Form
$form.Text = "Attivazione WSL"
$form.Size = New-Object System.Drawing.Size(350,200) 
$form.StartPosition = "CenterScreen"
$form.FormBorderStyle = 'FixedDialog'

# Set form background color to dark
$form.BackColor = [System.Drawing.Color]::FromArgb(40, 40, 40)

# Set text color to white
$form.ForeColor = [System.Drawing.Color]::White

# Create x64 or x32
$groupBoxDistro = New-Object System.Windows.Forms.GroupBox
$groupBoxDistro.Location = New-Object System.Drawing.Point(10,20)
$groupBoxDistro.Size = New-Object System.Drawing.Size(290,100)
$groupBoxDistro.Text = "Seleziona quale distro Linux usare"
$groupBoxDistro.ForeColor = [System.Drawing.Color]::White
$form.Controls.Add($groupBoxDistro)

$radioButtonUbuntu = New-Object System.Windows.Forms.RadioButton
$radioButtonUbuntu.Location = New-Object System.Drawing.Point(10,20)
$radioButtonUbuntu.Size = New-Object System.Drawing.Size(120,20)
$radioButtonUbuntu.Text = "Ubuntu"
$groupBoxDistro.Controls.Add($radioButtonUbuntu)

$radioButtonDebian = New-Object System.Windows.Forms.RadioButton
$radioButtonDebian.Location = New-Object System.Drawing.Point(10,45)
$radioButtonDebian.Size = New-Object System.Drawing.Size(120,20)
$radioButtonDebian.Text = "Debian"
$groupBoxDistro.Controls.Add($radioButtonDebian)

$radioButtonKaliLinux = New-Object System.Windows.Forms.RadioButton
$radioButtonKaliLinux.Location = New-Object System.Drawing.Point(140,20)
$radioButtonKaliLinux.Size = New-Object System.Drawing.Size(120,20)
$radioButtonKaliLinux.Text = "Kali Linux"
$groupBoxDistro.Controls.Add($radioButtonKaliLinux)

$radioButtonOpensuse = New-Object System.Windows.Forms.RadioButton
$radioButtonOpensuse.Location = New-Object System.Drawing.Point(140,45)
$radioButtonOpensuse.Size = New-Object System.Drawing.Size(120,20)
$radioButtonOpensuse.Text = "Opensuse"
$groupBoxDistro.Controls.Add($radioButtonOpensuse)

$radioButtonOracle = New-Object System.Windows.Forms.RadioButton
$radioButtonOracle.Location = New-Object System.Drawing.Point(66,75)
$radioButtonOracle.Size = New-Object System.Drawing.Size(120,20)
$radioButtonOracle.Text = "Oracle"
$groupBoxDistro.Controls.Add($radioButtonOracle)
    
# Create OK button
$buildButton = New-Object System.Windows.Forms.Button
$buildButton.Location = New-Object System.Drawing.Point(220,130) 
$buildButton.Size = New-Object System.Drawing.Size(100,23) 
$buildButton.Text = "Attiva WSL!"
$buildButton.Add_Click({

    if (-not ($radioButtonUbuntu.Checked -or $radioButtonDebian.Checked -or $radioButtonKaliLinux.Checked -or $radioButtonOpensuse.Checked -or $radioButtonOracle.Checked)) {
    [System.Windows.Forms.MessageBox]::Show("Seleziona la Distro desiderata.", "Error", [System.Windows.Forms.MessageBoxButtons]::OK, [System.Windows.Forms.MessageBoxIcon]::Error)
     return
    }

    if ($radioButtonUbuntu.Checked) {
        $Distro = "Ubuntu"
    } elseif ($radioButtonDebian.Checked) {
        $Distro = "Debian"
    } elseif ($radioButtonKaliLinux.Checked) {
        $Distro = "KaliLinux"
    } elseif ($radioButtonOpensuse.Checked) {
        $Distro = "Opensuse"
    } elseif ($radioButtonOracle.Checked) {
        $Distro = "Oracle"
    }

    Start-Process powershell -ArgumentList "wsl --install; dism.exe /online /enable-feature /featurename:Microsoft-Windows-Subsystem-Linux /all /norestart; dism.exe /online /enable-feature /featurename:VirtualMachinePlatform /all /norestart; wsl --set-default-version 2" -Verb RunAs -Wait

    switch ($Distro) {
        "Ubuntu" { Start-Process powershell -ArgumentList "wsl --install -d Ubuntu" -Verb RunAs -Wait }
        "Debian" { Start-Process powershell -ArgumentList "wsl --install -d Debian" -Verb RunAs -Wait }
        "KaliLinux" { Start-Process powershell -ArgumentList "wsl --install -d kali-linux" -Verb RunAs -Wait }
        "Opensuse" { Start-Process powershell -ArgumentList "wsl --install -d opensuse-leap-15.5" -Verb RunAs -Wait }
        "Oracle" { Start-Process powershell -ArgumentList "wsl --install -d oraclelinux_9_1" -Verb RunAs -Wait }
    }

    [System.Windows.Forms.MessageBox]::Show("Installazione completata. Per favore, riavvia il computer.", "Installazione WSL", [System.Windows.Forms.MessageBoxButtons]::OK, [System.Windows.Forms.MessageBoxIcon]::Information)
})
$form.Controls.Add($buildButton)

$form.Add_Shown({$form.Activate()})
$form.ShowDialog() | Out-Null
####################################