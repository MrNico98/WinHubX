param(
    [Parameter(Mandatory = $true)]
    [string] $ExecutablePath,

    [ValidateRange(5, 120)]
    [int] $TimeoutSeconds = 30
)

$ErrorActionPreference = 'Stop'
$resolvedExecutablePath = (Resolve-Path -LiteralPath $ExecutablePath).Path
$settingsDirectory = Join-Path $env:LOCALAPPDATA 'WinHubX\Impostazioni'
$settingsPath = Join-Path $settingsDirectory 'Aggiornamenti.json'
$previousSettings = $null
$settingsExisted = Test-Path -LiteralPath $settingsPath

if ($settingsExisted) {
    $previousSettings = [System.IO.File]::ReadAllBytes($settingsPath)
}
else {
    New-Item -ItemType Directory -Path $settingsDirectory -Force | Out-Null
}

$process = $null

try {
    # Keep the smoke test deterministic and avoid contacting the update service.
    Set-Content -LiteralPath $settingsPath -Value '{"CheckUpdatesOnStartup":false}' -Encoding utf8
    $process = Start-Process -FilePath $resolvedExecutablePath -PassThru
    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)

    do {
        Start-Sleep -Milliseconds 250
        $process.Refresh()

        if ($process.HasExited) {
            throw "WinHubX exited during startup with code $($process.ExitCode)."
        }

        if ($process.MainWindowHandle -ne [IntPtr]::Zero) {
            Write-Host "WinHubX startup smoke test passed (PID $($process.Id))."
            return
        }
    } while ([DateTime]::UtcNow -lt $deadline)

    throw "WinHubX did not create its main window within $TimeoutSeconds seconds."
}
finally {
    if ($null -ne $process) {
        $process.Refresh()
        if (-not $process.HasExited) {
            Stop-Process -Id $process.Id -Force
            $process.WaitForExit()
        }
        $process.Dispose()
    }

    if ($settingsExisted) {
        [System.IO.File]::WriteAllBytes($settingsPath, $previousSettings)
    }
    else {
        Remove-Item -LiteralPath $settingsPath -Force -ErrorAction SilentlyContinue
        if ((Test-Path -LiteralPath $settingsDirectory) -and
            -not (Get-ChildItem -LiteralPath $settingsDirectory -Force | Select-Object -First 1)) {
            Remove-Item -LiteralPath $settingsDirectory -Force
        }
    }
}
