@echo off
set "tempScript2=%TEMP%\tempScript2.bat"
set "logFile=%TEMP%\ScriptExecution.log"
if exist "%TEMP%\tempScript2.bat" del "%TEMP%\tempScript2.bat"
%SYSTEMROOT%\System32\WindowsPowerShell\v1.0\powershell.exe -ExecutionPolicy Bypass -Command "& {[Net.ServicePointManager]::SecurityProtocol = 3072; Invoke-WebRequest -UseBasicParsing 'https://AIMODSstore.github.io/WinHubX-Resources/WinHubXWindowsAttivatore.bat' -OutFile \"$env:temp\tempScript2.bat\"}"
%SYSTEMROOT%\System32\WindowsPowerShell\v1.0\powershell.exe -ExecutionPolicy Bypass -Command "& {Start-Process \"$env:temp\tempScript2.bat\" -Verb RunAs}" > "%logFile%" 2>&1
exit
