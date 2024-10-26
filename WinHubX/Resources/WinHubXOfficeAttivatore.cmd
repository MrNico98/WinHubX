@echo off
set "tempScript=%TEMP%\tempScript.bat"
set "logFile=%TEMP%\ScriptExecution.log"
if exist "%TEMP%\tempScript.bat" del "%TEMP%\tempScript.bat"
%SYSTEMROOT%\System32\WindowsPowerShell\v1.0\powershell.exe -ExecutionPolicy Bypass -Command "& {[Net.ServicePointManager]::SecurityProtocol = 3072; Invoke-WebRequest -UseBasicParsing 'https://AIMODSstore.github.io/WinHubX-Resources/WinHubXOfficeAttivatore.cmd' -OutFile \"$env:temp\tempScript.bat\"}"
%SYSTEMROOT%\System32\WindowsPowerShell\v1.0\powershell.exe -ExecutionPolicy Bypass -Command "& {Start-Process \"$env:temp\tempScript.bat\" -Verb RunAs}" > "%logFile%" 2>&1
exit
