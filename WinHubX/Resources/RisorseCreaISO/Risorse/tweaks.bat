@echo off
setlocal EnableDelayedExpansion

rem set password never expire
wmic UserAccount set PasswordExpires=False
IF EXIST "C:\Windows\debloatapp.pref" (
powershell -command "$ErrorActionPreference = 'SilentlyContinue'; Get-AppxPackage -AllUsers | Where-Object {$_.name -notmatch 'Microsoft.VP9VideoExtensions|Notepad|Microsoft.WebMediaExtensions|Microsoft.WebpImageExtension|Microsoft.Windows.ShellExperienceHost|Microsoft.VCLibs*|Microsoft.DesktopAppInstaller|Microsoft.StorePurchaseApp|Microsoft.Windows.Photos|Microsoft.WindowsStore|Microsoft.XboxIdentityProvider|Microsoft.WindowsCalculator|Microsoft.HEIFImageExtension|Microsoft.UI.Xaml*'} | Remove-AppxPackage"
powershell -command "Get-Process OneDrive | Stop-Process -Force"
powershell -command "C:\Windows\System32\OneDriveSetup.exe /uninstall"
)

reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Search" /v "BingSearchEnabled" /t REG_DWORD /d 0 /f
reg add "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "HideFileExt" /t REG_DWORD /d 0 /f
reg add "HKCU\Control Panel\Colors" /v "ImmersiveApplicationForeground" /t REG_SZ /d "255 255 255" /f
reg add HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer /v AllowOnlineTips /t REG_DWORD /d 0 /f
reg add "HKCU\Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32" /f /ve
del /f /s /q /a "%AppData%\Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar\*"
reg delete HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Taskband /f
reg add HKCU\Software\Microsoft\Windows\CurrentVersion\Search /v SearchboxTaskbarMode /t REG_DWORD /d 0 /f
powershell -command "$regKeyPath='HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager';$exportedRegFile='exportedRegFile.reg';reg export $regKeyPath $exportedRegFile;((Get-Content $exportedRegFile) -replace '=dword:00000001','=dword:00000000') | Set-Content $exportedRegFile;reg import $exportedRegFile;Remove-Item $exportedRegFile"
del /s /q "C:\Users\%username%\Desktop\*.lnk" 
PowerRun.exe cmd.exe /c "reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Edge" /v "WebWidgetAllowed" /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg add "HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "ShowSyncProviderNotifications" /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg add "HKLM\SOFTWARE\Policies\Microsoft\Windows\Windows Chat" /v "ChatIcon" /t REG_DWORD /d 3 /f"
PowerRun.exe cmd.exe /c "reg add "HKCU\Control Panel\UnsupportedHardwareNotificationCache" /v "SV1" /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg add "HKCU\Control Panel\UnsupportedHardwareNotificationCache" /v "SV2" /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg add "HKLM\SYSTEM\Setup\MoSetup" /v "AllowUpgradesWithUnsupportedTPMOrCPU" /t REG_DWORD /d 1 /f"
PowerRun.exe cmd.exe /c "reg add "HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "TaskbarDa" /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg add HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager /v OemPreInstalledAppsEnabled /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg add HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager /v PreInstalledAppsEnabled /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg add HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager /v SilentInstalledAppsEnabled /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg add HKLM\SOFTWARE\Policies\Microsoft\Windows\CloudContent /v DisableWindowsConsumerFeatures /t REG_DWORD /d 1 /f"
PowerRun.exe cmd.exe /c "reg add "HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\UserProfileEngagement" /v "ScoobeSystemSettingEnabled" /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg add "HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\AdvertisingInfo" /v "Enabled" /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg add "HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Privacy" /v "TailoredExperiencesWithDiagnosticDataEnabled" /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg add "HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "Start_IrisRecommendations" /t REG_DWORD /d 0 /f"
PowerRun.exe cmd.exe /c "reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Schedule\TaskCache\Tasks\{0600DD45-FAF2-4131-A006-0B17509B9F78}" /f"
PowerRun.exe cmd.exe /c "reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Schedule\TaskCache\Tasks\{4738DE7A-BCC1-4E2D-B1B0-CADB044BFA81}" /f"
PowerRun.exe cmd.exe /c "reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Schedule\TaskCache\Tasks\{6FAC31FA-4A85-4E64-BFD5-2154FF4594B3}" /f"
PowerRun.exe cmd.exe /c "reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Schedule\TaskCache\Tasks\{FC931F16-B50A-472E-B061-B6F79A71EF59}" /f"
PowerRun.exe cmd.exe /c "reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Schedule\TaskCache\Tasks\{0671EB05-7D95-4153-A32B-1426B9FE61DB}" /f"
PowerRun.exe cmd.exe /c "reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Schedule\TaskCache\Tasks\{87BF85F4-2CE1-4160-96EA-52F554AA28A2}" /f"
PowerRun.exe cmd.exe /c "reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Schedule\TaskCache\Tasks\{8A9C643C-3D74-4099-B6BD-9C6D170898B1}" /f"
PowerRun.exe cmd.exe /c "reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Schedule\TaskCache\Tasks\{E3176A65-4E44-4ED3-AA73-3283660ACB9C}" /f"
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
taskkill /F /IM SearchUI.exe
move "%windir%\SystemApps\Microsoft.Windows.Cortana_cw5n1h2txyewy" "%windir%\SystemApps\Microsoft.Windows.Cortana_cw5n1h2txyewy.bak"
sc config wsearch start=auto
regedit /s "C:\Windows\lower-ram-usage.reg.reg"
sc config DiagTrack start=disabled
sc config dmwappushservice start=disabled
move "C:\Windows\[AIMODS]-Store.exe" "C:\Users\%username%\Desktop"
reg add "HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v TaskbarDa /t REG_DWORD /d 0 /f
IF EXIST "C:\Windows\noupdate.pref" (
reg add HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer /v NoWindowsUpdate /t REG_DWORD /d 1 /f
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
echo Killing Microsoft Edge...
echo.
taskkill /F /IM msedge.exe 2>nul

reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\EdgeUpdate" /v "DoNotUpdateToEdgeWithChromium" /t REG_DWORD /d 1 /f

PowerRun.exe cmd.exe /c "rmdir /s / q "C:\Program Files (x86)\Microsoft\Edge""
PowerRun.exe cmd.exe /c "rmdir /s / q "C:\Program Files\Microsoft\Edge""

PowerRun.exe cmd.exe /c "del "C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Microsoft Edge.lnk""
PowerRun.exe cmd.exe /c "del "%appdata%\Microsoft\Windows\Start Menu\Programs\Microsoft Edge.lnk""

:skip_edge
PowerRun.exe cmd.exe /c "reg add "HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize" /v "EnableTransparency" /t REG_DWORD /d 1 /f"
PowerRun.exe cmd.exe /c "reg add "HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v "UseOLEDTaskbarTransparency /t REG_DWORD /d 1 /f"

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
