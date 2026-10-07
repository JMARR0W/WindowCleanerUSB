@echo off
setlocal

echo Windows Reinstaller - NON-DESTRUCTIVE HARDWARE TEST
echo This script only reads hardware information.
echo.
if not exist X:\Logs md X:\Logs
powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File "%~dp0DetectHardware.ps1"
echo.
echo Detection finished. No disk changes were made.
pause
exit /b %ERRORLEVEL%
