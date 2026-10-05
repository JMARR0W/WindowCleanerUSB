[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$WindowsIso,

    [Parameter(Mandatory)]
    [ValidatePattern('^[A-Za-z]:$')]
    [string]$UsbDrive
)

$ErrorActionPreference = 'Stop'

Write-Host "USB build scaffold."
Write-Host "Windows ISO: $WindowsIso"
Write-Host "USB target : $UsbDrive"
Write-Host ""
Write-Host "No USB contents will be modified by this initial scaffold."
