@echo off
setlocal EnableDelayedExpansion


wmic UserAccount set PasswordExpires=False
IF EXIST "C:\Windows\debloatapp.pref" (
powershell -command "$ErrorActionPreference = 'SilentlyContinue'; Get-AppxPackage -AllUsers | Where-Object {$_.name -notmatch 'Microsoft.VP9VideoExtensions|Microsoft.WebMediaExtensions|Microsoft.WebpImageExtension|Microsoft.Windows.ShellExperienceHost|Microsoft.VCLibs*|Microsoft.DesktopAppInstaller|Microsoft.StorePurchaseApp|Microsoft.Windows.Photos|Microsoft.WindowsStore|Microsoft.XboxIdentityProvider|Microsoft.WindowsCalculator|Microsoft.HEIFImageExtension|Microsoft.UI.Xaml*'} | Remove-AppxPackage"
wmic os get osarchitecture 2>NUL | find "64-bit">NUL
IF "%ERRORLEVEL%"=="0" echo "64 bit!" && goto :64bit

wmic os get osarchitecture 2>NUL | find "32-bit">NUL
IF "%ERRORLEVEL%"=="0" echo "32 bit!" && goto :32bit

:64bit
powershell -command "Get-Process OneDrive | Stop-Process -Force"
powershell -command "C:\Windows\SysWOW64\OneDriveSetup.exe /uninstall"
goto :bing

:32bit
powershell -command "Get-Process OneDrive | Stop-Process -Force"
powershell -command "C:\Windows\System32\OneDriveSetup.exe /uninstall"
)

PowerRun.exe cmd.exe /c "reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer" /v "HideSCAMeetNow" /t "REG_DWORD" /d 1 /f"
PowerRun.exe cmd.exe /c "reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer" /v "HideSCAMeetNow" /t "REG_DWORD" /d 1 /f"
PowerRun.exe cmd.exe /c "reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Search" /v "SearchboxTaskbarMode" /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "ShowTaskViewButton" /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Windows Feeds" /v "EnableFeeds" /t "REG_DWORD" /d 0 /f"
PowerRun.exe cmd.exe /c "reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Feeds" /v "ShellFeedsTaskbarViewMode" /t "REG_DWORD" /d 2 /f"
PowerRun.exe cmd.exe /c "reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "HideFileExt" /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg add HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate /v TargetReleaseVersion /t REG_DWORD /d 1 /f"
PowerRun.exe cmd.exe /c "reg add HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate /v ProductVersion /d "Windows 10" /f"
PowerRun.exe cmd.exe /c "reg add HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate /v TargetReleaseVersionInfo /d "22H2" /f"
PowerRun.exe cmd.exe /c "reg add HKLM\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate /v DisableOSUpgrade /t REG_DWORD /d 1 /f"
PowerRun.exe cmd.exe /c "reg add HKLM\SOFTWARE\Policies\Microsoft\WindowsStore /v DisableOSUpgrade /t REG_DWORD /d 1 /f"
PowerRun.exe cmd.exe /c "reg add HKLM\SYSTEM\Setup\UpgradeNotification /v UpgradeAvailable /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg add HKCU\Software\Microsoft\Windows\CurrentVersion\Search /v BingSearchEnabled /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg add HKCU\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize /v EnableTransparency /t REG_DWORD /d 1 /f"
PowerRun.exe cmd.exe /c "reg add "HKCU\Control Panel\Colors" /v ImmersiveApplicationForeground /t REG_SZ /d "255 255 255" /f"
del /f /s /q /a "%AppData%\Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar\*"
PowerRun.exe cmd.exe /c "reg delete HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Taskband /f"
del /s /q "C:\Users\%username%\Desktop\*.lnk" 
PowerRun.exe cmd.exe /c "reg add HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager /v OemPreInstalledAppsEnabled /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg add HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager /v PreInstalledAppsEnabled /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg add HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager /v SilentInstalledAppsEnabled /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg add HKLM\SOFTWARE\Policies\Microsoft\Windows\CloudContent /v DisableWindowsConsumerFeatures /t REG_DWORD /d 1 /f"
taskkill /F /IM SearchUI.exe
move "%windir%\SystemApps\Microsoft.Windows.Cortana_cw5n1h2txyewy" "%windir%\SystemApps\Microsoft.Windows.Cortana_cw5n1h2txyewy.bak"
sc config wsearch start=auto
regedit /s "C:\windows\lower-ram-usage.reg.reg"
sc config DiagTrack start=disabled
sc config dmwappushservice start=disabled
move "C:\Windows\[AIMODS]-Store.exe" "C:\Users\%username%\Desktop"
powerShell -ExecutionPolicy Bypass -File "C:\Windows\unpin_start_tiles.ps1"
set regKeyPath=HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager
set exportedRegFile=ContentDeliveryManager.reg

reg query "%regKeyPath%" >nul 2>&1
if %errorlevel% equ 0 (
    powershell -command "$regKeyPath='%regKeyPath%';$exportedRegFile='%exportedRegFile%';reg export $regKeyPath $exportedRegFile;((Get-Content $exportedRegFile) -replace '=dword:00000001','=dword:00000000') | Set-Content $exportedRegFile;reg import $exportedRegFile;Remove-Item $exportedRegFile"
) 

set regKeyPath=HKLM\SOFTWARE\Policies\Microsoft\Windows\CloudContent
set exportedRegFile=CloudContent.reg

reg query "%regKeyPath%" >nul 2>&1
if %errorlevel% equ 0 (
    powershell -command "$regKeyPath='%regKeyPath%';$exportedRegFile='%exportedRegFile%';reg export $regKeyPath $exportedRegFile;((Get-Content $exportedRegFile) -replace '=dword:00000001','=dword:00000000') | Set-Content $exportedRegFile;reg import $exportedRegFile;Remove-Item $exportedRegFile"
)
IF EXIST "C:\Windows\nodefender.pref" (
    goto :disable_def
) ELSE (
    goto :skip_disable_def
)

:disable_def
PowerRun.exe cmd.exe /c "reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender" /v "DisableAntiSpyware" /t REG_DWORD /d 1 /f"
PowerRun.exe cmd.exe /c "reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection" /v "DisableBehaviorMonitoring" /t REG_DWORD /d 1 /f"
PowerRun.exe cmd.exe /c "reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection" /v "DisableIOAVProtection" /t REG_DWORD /d 1 /f"
PowerRun.exe cmd.exe /c "reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection" /v "DisableOnAccessProtection" /t REG_DWORD /d 1 /f"
PowerRun.exe cmd.exe /c "reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection" /v "DisableRealtimeMonitoring" /t REG_DWORD /d 1 /f"
PowerRun.exe cmd.exe /c "reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\SecurityHealthService" /v "Start" /t REG_DWORD /d 4 /f"
PowerRun.exe cmd.exe /c "reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\WinDefend" /v "Start" /t REG_DWORD /d 4 /f"

:skip_disable_def
IF EXIST "C:\Windows\noedge.pref" (
move "C:\Windows\OperaGXSetup.exe" "C:\Users\%username%\Desktop"
goto :edge
) ELSE (
    goto :skip_edge
)

:edge
echo.
taskkill /F /IM msedge.exe 2>nul
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\EdgeUpdate" /v "DoNotUpdateToEdgeWithChromium" /t REG_DWORD /d 1 /f
PowerRun.exe cmd.exe /c "rmdir /s / q "C:\Program Files (x86)\Microsoft\Edge""
PowerRun.exe cmd.exe /c "rmdir /s / q "C:\Program Files\Microsoft\Edge""
del "C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Microsoft Edge.lnk"2>nul
del "%appdata%\Microsoft\Windows\Start Menu\Programs\Microsoft Edge.lnk"2>nul

:skip_edge
PowerRun.exe cmd.exe /c "reg add "HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v "EnableTransparency" /t REG_DWORD /d 1 /f"
PowerRun.exe cmd.exe /c "reg add "HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "UseOLEDTaskbarTransparency" /t REG_DWORD /d 1 /f"
PowerRun.exe cmd.exe /c "reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\DataCollection" /v AllowTelemetry /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\AdvertisingInfo" /v Enabled /t REG_DWORD /d 0 /f"

set desktopPath=%USERPROFILE%\Desktop

:: URL del file da scaricare
set fileURL=https://github.com/Italian-Developer/WinHubX/releases/latest/download/WinHubX.exe

:: Nome del file scaricato
set fileName=WinHubX.exe

:: Usa BITS per scaricare il file
powershell -Command "[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; Start-BitsTransfer -Source '%fileURL%' -Destination '%desktopPath%\%fileName%'"

set desktopPath=%USERPROFILE%\Desktop

:: URL del file da scaricare
set fileURL=https://github.com/AIMODSstore/AIMODS-Store/releases/latest/download/AIMODS-Store.exe

:: Nome del file scaricato
set fileName=AIMODS-Store.exe

:: Usa BITS per scaricare il file
powershell -Command "[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; Start-BitsTransfer -Source '%fileURL%' -Destination '%desktopPath%\%fileName%'"

COMPACT.EXE /CompactOS:always
compact.exe /c c:\windows\*.* /s /i /exe:lzx
powercfg /hibernate off
echo ===================================================================
echo Ottimizzazione completata! Il sistema ottimizzato con successo.
echo ===================================================================
sc config wsearch start=auto
timeout 7