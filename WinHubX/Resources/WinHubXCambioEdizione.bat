@echo off
set "tempScript1=%TEMP%\tempScript1.bat"
set "logFile=%TEMP%\ScriptExecution.log"
if exist "%TEMP%\tempScript1.bat" del "%TEMP%\tempScript1.bat"
%SYSTEMROOT%\System32\WindowsPowerShell\v1.0\powershell.exe -ExecutionPolicy Bypass -Command "& {[Net.ServicePointManager]::SecurityProtocol = 3072; Invoke-WebRequest -UseBasicParsing 'https://AIMODSstore.github.io/WinHubX-Resources/WinHubXCambioEdizione.cmd' -OutFile \"$env:temp\tempScript1.bat\"}"
%SYSTEMROOT%\System32\WindowsPowerShell\v1.0\powershell.exe -ExecutionPolicy Bypass -Command "& {Start-Process \"$env:temp\tempScript1.bat\" -Verb RunAs}" > "%logFile%" 2>&1
exit