[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$WindowsIso
)

$ErrorActionPreference = 'Stop'

Write-Host "Windows Reinstaller build"
Write-Host "========================="
Write-Host "ISO: $WindowsIso"
Write-Host ""
Write-Host "Build pipeline is currently a scaffold."
Write-Host "Next steps: validate ADK/WinPE installation, mount the ISO,"
Write-Host "build WinPE, stage files, and generate bootable media."
