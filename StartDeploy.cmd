@echo off
setlocal

echo Windows Reinstaller WinPE environment
echo Deployment engine is currently a scaffold.
echo.

powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0Deploy.ps1"

exit /b %ERRORLEVEL%
