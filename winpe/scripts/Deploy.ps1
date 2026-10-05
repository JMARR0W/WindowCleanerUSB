$ErrorActionPreference = 'Stop'

Write-Host "Windows Reinstaller deployment environment"
Write-Host "========================================="
Write-Host ""
Write-Host "This initial version performs detection only."
Write-Host "No disks will be modified."
Write-Host ""

& "$PSScriptRoot\DetectHardware.ps1"

Write-Host ""
Write-Host "Deployment operations are intentionally not implemented yet."
