$ErrorActionPreference = 'Continue'
$logDirectory = 'X:\Logs'
$logPath = Join-Path $logDirectory 'HardwareReport.txt'
New-Item -ItemType Directory -Path $logDirectory -Force | Out-Null

function Write-ReportSection {
    param([string]$Title, [string]$ClassName, [string[]]$Properties)

    Add-Content -LiteralPath $logPath -Value "`r`n=== $Title ==="
    Write-Host "`n=== $Title ==="
    try {
        $items = @(Get-WmiObject -Class $ClassName -ErrorAction Stop | Select-Object -Property $Properties)
        if ($items.Count -eq 0) {
            $lines = @('(No devices found.)')
        } else {
            $lines = @($items | Format-List | Out-String -Stream)
        }
        foreach ($line in $lines) {
            Add-Content -LiteralPath $logPath -Value $line
            Write-Host $line
        }
    } catch {
        $message = "Unable to query ${ClassName}: $($_.Exception.Message)"
        Add-Content -LiteralPath $logPath -Value $message
        Write-Warning $message
    }
}

Set-Content -LiteralPath $logPath -Value @(
    'Windows Reinstaller - Non-Destructive WinPE Hardware Report'
    "Captured: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
    'This script only reads hardware information and writes this report to the WinPE RAM drive.'
    'It does not write to, partition, format, or install to any disk.'
) -Encoding UTF8

Write-Host 'Windows Reinstaller - NON-DESTRUCTIVE HARDWARE TEST'
Write-Host 'No disks will be changed. Collecting hardware information...'

$firmware = 'Unknown'
try {
    $firmwareType = (Get-ItemProperty -Path 'HKLM:\SYSTEM\CurrentControlSet\Control' -Name PEFirmwareType -ErrorAction Stop).PEFirmwareType
    if ($firmwareType -eq 1) { $firmware = 'Legacy BIOS' }
    elseif ($firmwareType -eq 2) { $firmware = 'UEFI' }
} catch {
    $firmware = "Unavailable ($($_.Exception.Message))"
}
Add-Content -LiteralPath $logPath -Value "`r`n=== Firmware ===`r`nBoot mode: $firmware"
Write-Host "`n=== Firmware ===`nBoot mode: $firmware"

Write-ReportSection 'Computer system' 'Win32_ComputerSystem' @('Manufacturer', 'Model', 'SystemType', 'TotalPhysicalMemory')
Write-ReportSection 'BIOS' 'Win32_BIOS' @('Manufacturer', 'SMBIOSBIOSVersion', 'SerialNumber', 'ReleaseDate')
Write-ReportSection 'Processor' 'Win32_Processor' @('Name', 'Manufacturer', 'NumberOfCores', 'NumberOfLogicalProcessors')
Write-ReportSection 'Physical disks (read-only inventory)' 'Win32_DiskDrive' @('Index', 'Model', 'MediaType', 'InterfaceType', 'Size', 'Status', 'SerialNumber')
Write-ReportSection 'Storage controllers' 'Win32_SCSIController' @('Name', 'Manufacturer', 'DeviceID', 'PNPDeviceID')
Write-ReportSection 'Network adapters' 'Win32_NetworkAdapter' @('Name', 'Manufacturer', 'PhysicalAdapter', 'NetEnabled', 'PNPDeviceID')

Write-Host "`nReport saved to $logPath"
Write-Host 'Detection finished. No disk changes were made.'
