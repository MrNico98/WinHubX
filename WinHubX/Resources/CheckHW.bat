@echo off
chcp 437 > nul
set LANG=en_US.UTF-8
setlocal enabledelayedexpansion


for /f %%I in ('powershell -command "Get-Partition -DriveLetter C | Get-Disk | Get-PhysicalDisk | Where-Object MediaType -ne $null | Select-Object -ExpandProperty MediaType"') do (
    set "mediatype=%%I"
    goto :DisplayMediaType
)

:DisplayMediaType
if not defined mediatype (
powerShell -Command "Write-Host 'Non trovo il tipo di disco' -ForegroundColor Red; exit" && timeout 04>nul
)


if "%mediatype%"=="SSD" (
 powerShell -Command "Write-Host 'Il disco del PC risulta essere un SSD, adatto per i nuovi SO di Win10/11' -ForegroundColor Green; exit" && timeout 04>nul
 set "disk=+"
) else (
 powerShell -Command "Write-Host 'Il disco del PC risulta essere un HDD, non adatto per i nuovi SO di Win10/11' -ForegroundColor Red; exit" && timeout 02>nul
 powerShell -Command "Write-Host 'Appena possibile sostituiscilo con un SSD' -ForegroundColor Red; exit" && timeout 04>nul
 set "disk=-"
)

echo.
echo.

for /f "delims=" %%I in ('powershell "Get-WmiObject -Class Win32_Processor | Select-Object -ExpandProperty Name"') do (
    set "processor=%%I"
)

echo !processor! | find /i "i3" >nul
if !errorlevel! equ 0 (
    set "numbercpu=i5"
    set "cpu=+"
    set "numero_cpu=!processor:Intel(R) Core(TM) i5=!"
    set "numero_cpu=!numero_cpu:-=!"
    set "primi_due_numeri=!numero_cpu:~0,1!"
    goto :checkcpu
) else (
    goto :intel5
)

:intel5
echo !processor! | find /i "i5" >nul
if !errorlevel! equ 0 (
    set "numbercpu=i5"
    set "cpu=+"
    set "numero_cpu=!processor:Intel(R) Core(TM) i5=!"
    set "numero_cpu=!numero_cpu:-=!"
    set "primi_due_numeri=!numero_cpu:~0,1!"
    goto :checkcpu
) else (
    goto :intel7
)

:intel7
echo !processor! | find /i "i7" >nul
if !errorlevel! equ 0 (
    set "numbercpu=i7"
    set "cpu=+"
    set "numero_cpu=!processor:Intel(R) Core(TM) i7=!"
    set "numero_cpu=!numero_cpu:-=!"
    set "primi_due_numeri=!numero_cpu:~0,1!"
    goto :checkcpu
) else (
    goto :intel9
)

:intel9
echo !processor! | find /i "i9" >nul
if !errorlevel! equ 0 (
    set "numbercpu=i9"
    set "cpu=+"
    set "numero_cpu=!processor:Intel(R) Core(TM) i9=!"
    set "numero_cpu=!numero_cpu:-=!"
    set "primi_due_numeri=!numero_cpu:~0,1!"
    goto :checkcpu
) else (
    goto :intelxeon
)

:intelxeon
echo !processor! | find /i "Xeon" >nul
if !errorlevel! equ 0 (
    set "numbercpu=Xeon"
    set "cpu=+"
    set "numero_cpu=!processor:Intel(R) Core(TM) Xeon=!"
    set "numero_cpu=!numero_cpu:-=!"
    set "primi_due_numeri=!numero_cpu:~0,1!"
    goto :checkcpu
) else (
    goto :intelrestanti
)

:intelrestanti
echo !processor! | find /i "x6211E" >nul
echo !processor! | find /i "x6212RE" >nul
echo !processor! | find /i "x6413E" >nul
echo !processor! | find /i "x6414RE" >nul
echo !processor! | find /i "x6425E" >nul
echo !processor! | find /i "x6425RE" >nul
echo !processor! | find /i "x6427FE" >nul
echo !processor! | find /i "6305" >nul
echo !processor! | find /i "7300" >nul
echo !processor! | find /i "7305" >nul
echo !processor! | find /i "3867U" >nul
echo !processor! | find /i "4205U" >nul
echo !processor! | find /i "4305U" >nul
echo !processor! | find /i "4305UE" >nul
echo !processor! | find /i "5205U" >nul
echo !processor! | find /i "5305U" >nul
echo !processor! | find /i "6305E" >nul
echo !processor! | find /i "6600HE" >nul
echo !processor! | find /i "7305E" >nul
echo !processor! | find /i "7305L" >nul
echo !processor! | find /i "G4900" >nul
echo !processor! | find /i "G4900T" >nul
echo !processor! | find /i "G4920" >nul
echo !processor! | find /i "G4930" >nul
echo !processor! | find /i "G4930E" >nul
echo !processor! | find /i "G4930T" >nul
echo !processor! | find /i "G4932E" >nul
echo !processor! | find /i "G4950" >nul
echo !processor! | find /i "G5900" >nul
echo !processor! | find /i "G5900E" >nul
echo !processor! | find /i "G5900T" >nul
echo !processor! | find /i "G5900TE" >nul
echo !processor! | find /i "G5905" >nul
echo !processor! | find /i "G5905T" >nul
echo !processor! | find /i "G5920" >nul
echo !processor! | find /i "G5925" >nul
echo !processor! | find /i "G6900" >nul
echo !processor! | find /i "G6900E" >nul
echo !processor! | find /i "G6900T" >nul
echo !processor! | find /i "G6900TE" >nul
echo !processor! | find /i "J4005" >nul
echo !processor! | find /i "J4025" >nul
echo !processor! | find /i "J4105" >nul
echo !processor! | find /i "J4115" >nul
echo !processor! | find /i "J4125" >nul
echo !processor! | find /i "J6412" >nul
echo !processor! | find /i "J6413" >nul
echo !processor! | find /i "N4000" >nul
echo !processor! | find /i "N4020" >nul
echo !processor! | find /i "N4100" >nul
echo !processor! | find /i "N4120" >nul
echo !processor! | find /i "N4500" >nul
echo !processor! | find /i "N4505" >nul
echo !processor! | find /i "N5100" >nul
echo !processor! | find /i "N5105" >nul
echo !processor! | find /i "N6210" >nul
echo !processor! | find /i "N6211" >nul
if !errorlevel! equ 0 (
    set "numbercpu=PROCESSORE"
    set "cpu=+"
    set "primi_due_numeri=8"
    goto :checkcpu
) else (
    goto :amd3
)

:amd3
echo !processor! | find /i "Ryzen 3" >nul
if !errorlevel! equ 0 (
    set "numbercpu=Ryzen3"
    set "cpu=+"
    set "numero_cpu=!modello:AMD Ryzen 3=!"
    set "numero_cpu=!numero_cpu:-=!"
    set "primi_due_numeri=!numero_cpu:~0,1!"
    goto :checkcpu
) else (
    goto :amd5
)

:amd5
echo !processor! | find /i "Ryzen 5" >nul
if !errorlevel! equ 0 (
    set "numbercpu=Ryzen5"
    set "cpu=+"
    set "numero_cpu=!modello:AMD Ryzen 5=!"
    set "numero_cpu=!numero_cpu:-=!"
    set "primi_due_numeri=!numero_cpu:~0,1!"
    goto :checkcpu
) else (
    goto :amd7
)

:amd7
echo !processor! | find /i "Ryzen 7" >nul
if !errorlevel! equ 0 (
    set "numbercpu=Ryzen7"
    set "cpu=+"
    set "numero_cpu=!modello:AMD Ryzen 7=!"
    set "numero_cpu=!numero_cpu:-=!"
    set "primi_due_numeri=!numero_cpu:~0,1!"
    goto :checkcpu
) else (
    goto :amdthredripper
)

:amdthredripper
echo !processor! | find /i "Ryzen Threadripper" >nul
if !errorlevel! equ 0 (
    set "numbercpu=Threadripper"
    set "cpu=+"
    set "numero_cpu=!modello:AMD Ryzen Threadripper=!"
    set "numero_cpu=!numero_cpu:-=!"
    set "primi_due_numeri=!numero_cpu:~0,1!"
    goto :checkcpu
) else  (
    goto :altramd
)

:altramd
echo !processor! | find /i "3015e" >nul
echo !processor! | find /i "3020e" >nul
echo !processor! | find /i "3000G" >nul
echo !processor! | find /i "300GE" >nul
echo !processor! | find /i "300U" >nul
echo !processor! | find /i "320GE" >nul
echo !processor! | find /i "7120e" >nul
echo !processor! | find /i "7120U" >nul
echo !processor! | find /i "7220e" >nul
echo !processor! | find /i "7220U" >nul
echo !processor! | find /i "3150C" >nul
echo !processor! | find /i "3150G" >nul
echo !processor! | find /i "3150GE" >nul
echo !processor! | find /i "3150U" >nul
echo !processor! | find /i "3050C" >nul
echo !processor! | find /i "3050e" >nul
echo !processor! | find /i "3050GE" >nul
echo !processor! | find /i "3050U" >nul
echo !processor! | find /i "4150GE" >nul
echo !processor! | find /i "3045B" >nul
echo !processor! | find /i "7252" >nul
echo !processor! | find /i "7262" >nul
echo !processor! | find /i "7272" >nul
echo !processor! | find /i "7282" >nul
echo !processor! | find /i "7302" >nul
echo !processor! | find /i "7313" >nul
echo !processor! | find /i "7343" >nul
echo !processor! | find /i "7352" >nul
echo !processor! | find /i "7402" >nul
echo !processor! | find /i "7413" >nul
echo !processor! | find /i "7443" >nul
echo !processor! | find /i "7452" >nul
echo !processor! | find /i "7453" >nul
echo !processor! | find /i "7502" >nul
echo !processor! | find /i "7513" >nul
echo !processor! | find /i "7532" >nul
echo !processor! | find /i "7542" >nul
echo !processor! | find /i "7543" >nul
echo !processor! | find /i "7552" >nul
echo !processor! | find /i "7642" >nul
echo !processor! | find /i "7643" >nul
echo !processor! | find /i "7662" >nul
echo !processor! | find /i "7663" >nul
echo !processor! | find /i "7702" >nul
echo !processor! | find /i "7713" >nul
echo !processor! | find /i "7742" >nul
echo !processor! | find /i "7763" >nul
echo !processor! | find /i "7232P" >nul
echo !processor! | find /i "72F3" >nul
echo !processor! | find /i "7302P" >nul
echo !processor! | find /i "7313P" >nul
echo !processor! | find /i "73F3" >nul
echo !processor! | find /i "7402P" >nul
echo !processor! | find /i "7443P" >nul
echo !processor! | find /i "74F3" >nul
echo !processor! | find /i "7502P" >nul
echo !processor! | find /i "7543P" >nul
echo !processor! | find /i "75F3" >nul
echo !processor! | find /i "7702P" >nul
echo !processor! | find /i "7713P" >nul
echo !processor! | find /i "7F32" >nul
echo !processor! | find /i "7F52" >nul
echo !processor! | find /i "7F72" >nul
echo !processor! | find /i "7H12" >nul
if !errorlevel! equ 0 (
 set "numbercpu=PROCESSORE"
 set "cpu=+"
 set "primi_due_numeri=8"
 goto :checkcpu
) else (
 set "numbercpu=PROCESSORE"
 set "cpu=-"
 set "primi_due_numeri=5"
)

echo.
echo.

:checkcpu
if "%cpu%" equ "+" (
    powerShell -Command "Write-Host 'Il tuo %numbercpu% va bene per i nuovi SO Win10/11' -ForegroundColor Green; exit" && timeout 04>nul
)

if "%cpu%" equ "-" (
    powerShell -Command "Write-Host 'Il tuo %numbercpu% non va bene per i nuovi SO Win10/11' -ForegroundColor Red; exit" && timeout 04>nul
)

echo.
echo.

for /f %%A in ('powershell -Command "(Get-WmiObject Win32_PhysicalMemory | Measure-Object Capacity -Sum).Sum / 1GB | Select-Object -First 2"') do (
    set "totalMemoryGB=%%A"
)

if not defined totalMemoryGB (
    powershell -Command "Write-Host 'Non trovo la RAM' -ForegroundColor Red; exit" && timeout /t 4 >nul
) else (
    if %totalMemoryGB% LEQ 3 (
        set "ram=-"
        powershell -Command "Write-Host 'RAM insufficiente per i nuovi SO Win10/11, requisito minimo 4gb' -ForegroundColor Red; exit" && timeout /t 4 >nul
    ) else if %totalMemoryGB% EQU 4 (
        set "ram=+"
        powershell -Command "Write-Host 'RAM minima per i nuovi SO Win10/11, espandila se possibile' -ForegroundColor DarkYellow; exit" && timeout /t 4 >nul
    ) else if %totalMemoryGB% GEQ 5 (
        set "ram=ok"
        powershell -Command "Write-Host 'RAM sufficiente per i nuovi SO Win10/11' -ForegroundColor Green; exit" && timeout /t 4 >nul
    )
)

echo.
echo.
cls
if %primi_due_numeri% GEQ 8 (
    powershell -Command "Write-Host 'La tua CPU supporta Win11 senza bypassare requisiti' -ForegroundColor Green; exit" && timeout /t 4 >nul
) else if %primi_due_numeri% LEQ 7 (
powershell -Command "Write-Host 'Devi bypassare i requisiti per installare Win11' -ForegroundColor DarkYellow; exit" && timeout /t 4 >nul
)

echo.
echo Scelta SO:
if "%cpu%" equ "-" (
    if "%disk%" equ "-" (
        if "%ram%" equ "-" (
            powershell -Command "Write-Host Il tuo PC fatica a girare su Win10/11 a causa della CPU, RAM e HDD' -ForegroundColor Red; exit"
            powershell -Command "Write-Host 'Puoi provare con Win8.1 Lite e vedere come va' -ForegroundColor DarkYellow; exit"
            powershell -Command "Write-Host 'SO consigliato Win7/Win8.1' -ForegroundColor Green; exit"  
        )
    )
)

if "%cpu%" equ "+" (
    if "%disk%" equ "-" (
        if "%ram%" equ "-" (
            powershell -Command "Write-Host 'Il tuo PC fatica a girare su Win11 a causa del HDD ed e della poca RAM' -ForegroundColor Red; exit"
            powershell -Command "Write-Host 'Puoi provare con Win11 Lite e vedere come va' -ForegroundColor DarkYellow; exit"
            powershell -Command "Write-Host 'SO consigliato Win8.1/10 Lite' -ForegroundColor Green; exit"  
        ) 
    )
)

if "%cpu%" equ "-" (
    if "%disk%" equ "+" (
        if "%ram%" equ "-" (
            powershell -Command "Write-Host 'Il tuo PC fatica a girare su Win11 a causa della CPU e della poca RAM' -ForegroundColor Red; exit"
            powershell -Command "Write-Host 'Puoi provare con Win11 Lite e vedere come va' -ForegroundColor DarkYellow; exit"
            powershell -Command "Write-Host 'SO consigliato Win10/10 Lite' -ForegroundColor Green; exit"
        )   
    )
)

if "%cpu%" equ "+" (
    if "%disk%" equ "+" (
        if "%ram%" equ "-" (
            powershell -Command "Write-Host 'Aumentare la RAM appena possibile, probabile causa di rallentamenti' -ForegroundColor DarkYellow; exit"
            powershell -Command "Write-Host 'SO consigliato Win10/11' -ForegroundColor Green; exit"
        )
    )
)

if "%cpu%" equ "-" (
    if "%disk%" equ "-" (
        if "%ram%" equ "+" (
            powershell -Command "Write-Host 'Il tuo PC fatica a girare su Win10/11 a causa della CPU e del HDD' -ForegroundColor Red; exit"
            powershell -Command "Write-Host 'Puoi provare con Win10 Lite e vedere come va' -ForegroundColor DarkYellow; exit"
            powershell -Command "Write-Host 'SO consigliato Win7/Win8.1' -ForegroundColor Green; exit"
        )
    )
)

if "%cpu%" equ "+" (
    if "%disk%" equ "-" (
        if "%ram%" equ "+" (
            powershell -Command "Write-Host 'Il tuo PC fatica a girare su Win10/11 a causa dell HDD' -ForegroundColor Red; exit"
            powershell -Command "Write-Host 'Puoi provare con Win10 Lite e vedere come va' -ForegroundColor DarkYellow; exit"
            powershell -Command "Write-Host 'SO consigliato Win8.1' -ForegroundColor Green; exit"
        )
    )
)

if "%cpu%" equ "-" (
    if "%disk%" equ "+" (
        if "%ram%" equ "+" (
            powershell -Command "Write-Host 'La CPU potrebbe causare dei rallentamenti' -ForegroundColor Red; exit"
            powershell -Command "Write-Host 'Puoi provare con Win10/11 e vedere come va' -ForegroundColor DarkYellow; exit"
            powershell -Command "Write-Host 'SO consigliato Win10/11 Lite' -ForegroundColor Green; exit"
        )
    )
)

if "%cpu%" equ "+" (
    if "%disk%" equ "+" (
        if "%ram%" equ "+" (
            powershell -Command "Write-Host 'Aumentare la RAM se possibile' -ForegroundColor DarkYellow; exit"
            powershell -Command "Write-Host 'SO consigliato Win10/11' -ForegroundColor Green; exit"
        )
    )
)

if "%cpu%" equ "+" (
    if "%disk%" equ "+" (
        if "%ram%" equ "ok" (
            powershell -Command "Write-Host 'SO consigliato Win10/11' -ForegroundColor Green; exit"
        )
    )
)

if "%cpu%" equ "-" (
    if "%disk%" equ "-" (
        if "%ram%" equ "ok" (
            powershell -Command "Write-Host 'Il tuo PC fatica a girare a girare su Win10/11 a causa della CPU del HDD' -ForegroundColor Red; exit"
            powershell -Command "Write-Host 'Puoi provare con Win8.1 e vedere come va' -ForegroundColor DarkYellow; exit"
            powershell -Command "Write-Host 'SO consigliato Win7 o Win8.1 Lite' -ForegroundColor Green; exit"  
        )
    )
)

if "%cpu%" equ "+" (
    if "%disk%" equ "-" (
        if "%ram%" equ "ok" (
            powershell -Command "Write-Host 'Il tuo PC fatica a girare su Win10/11 a causa del HDD' -ForegroundColor Red; exit"
            powershell -Command "Write-Host 'Puoi provare con Win10/11 Lite e vedere come va' -ForegroundColor DarkYellow; exit"
            powershell -Command "Write-Host 'SO consigliato Win8.1' -ForegroundColor Green; exit"  
        )
    )
)

if "%cpu%" equ "-" (
    if "%disk%" equ "+" (
        if "%ram%" equ "ok" (
            powershell -Command "Write-Host 'La CPU potrebbe causare dei rallentamenti' -ForegroundColor Red; exit"
            powershell -Command "Write-Host 'Puoi provare con Win10/11 e vedere come va' -ForegroundColor DarkYellow; exit"
            powershell -Command "Write-Host 'SO consigliato Win10/11 Lite' -ForegroundColor Green; exit"
        )
    )
)

echo.

echo Premi un tasto per uscire
pause >NUL
endlocal


