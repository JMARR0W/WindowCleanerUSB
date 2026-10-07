[CmdletBinding()]
param(
    [string]$WinPERoot = 'C:\WinPE_amd64',
    [string]$Architecture = 'amd64',
    [string]$Language = 'en-us'
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

function Invoke-Dism {
    param([Parameter(Mandatory)][string[]]$Arguments)
    & dism.exe @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "DISM failed with exit code ${LASTEXITCODE}: dism.exe $($Arguments -join ' ')"
    }
}

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$mediaRoot = Join-Path $WinPERoot 'media'
$imagePath = Join-Path $mediaRoot 'sources\boot.wim'
$scriptsSource = Join-Path $repoRoot 'winpe\scripts'
$optionalComponents = Join-Path ${env:ProgramFiles(x86)} "Windows Kits\10\Assessment and Deployment Kit\Windows Preinstallation Environment\$Architecture\WinPE_OCs"
$mountPath = Join-Path $env:TEMP 'WindowsReinstaller-WinPE-Mount'
$baselinePath = Join-Path $WinPERoot 'boot.wim.pristine'

if (-not (Test-Path $imagePath)) { throw "WinPE image not found: $imagePath. Run copype amd64 $WinPERoot first." }
if (-not (Test-Path $optionalComponents)) { throw "WinPE optional components not found: $optionalComponents. Install the ADK WinPE add-on for $Architecture." }
if (-not (Test-Path (Join-Path $optionalComponents "$Language\WinPE-PowerShell_$Language.cab"))) {
    throw "Language packages for '$Language' were not found under $optionalComponents."
}
if (-not (Test-Path (Join-Path $scriptsSource 'DetectHardware.ps1'))) { throw "Hardware detector not found under $scriptsSource" }
if (-not ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    throw 'Run this script from an elevated PowerShell or Deployment and Imaging Tools Environment window.'
}

$mounted = $false
try {
    $mountInfo = & dism.exe /Get-MountedWimInfo 2>&1 | Out-String
    if ($mountInfo -match [regex]::Escape($mountPath)) {
        throw "The build mount path is already registered with DISM: $mountPath. Inspect with dism /Get-MountedWimInfo."
    }
    if (Test-Path $mountPath) {
        if ((Get-ChildItem -Force $mountPath | Measure-Object).Count -ne 0) {
            throw "Mount directory must be empty: $mountPath"
        }
    } else {
        New-Item -ItemType Directory -Path $mountPath | Out-Null
    }

    # Preserve the original copype image once, then rebuild from it on every run.
    if (-not (Test-Path $baselinePath)) { Copy-Item $imagePath $baselinePath }
    Copy-Item $baselinePath $imagePath -Force

    Invoke-Dism @('/Mount-Image', '/ImageFile:' + $imagePath, '/Index:1', '/MountDir:' + $mountPath)
    $mounted = $true

    $componentNames = @('WinPE-WMI', 'WinPE-NetFX', 'WinPE-Scripting', 'WinPE-PowerShell')
    foreach ($component in $componentNames) {
        $cab = Join-Path $optionalComponents "$component.cab"
        $languageCab = Join-Path $optionalComponents "$Language\$component`_$Language.cab"
        if (-not (Test-Path $cab) -or -not (Test-Path $languageCab)) { throw "Required optional component package missing: $component ($Language)" }
        Invoke-Dism @('/Image:' + $mountPath, '/Add-Package', '/PackagePath:' + $cab)
        Invoke-Dism @('/Image:' + $mountPath, '/Add-Package', '/PackagePath:' + $languageCab)
    }

    $embeddedScripts = Join-Path $mountPath 'Windows\System32\WindowsReinstaller'
    New-Item -ItemType Directory -Path $embeddedScripts -Force | Out-Null
    Copy-Item (Join-Path $scriptsSource 'DetectHardware.ps1') (Join-Path $embeddedScripts 'DetectHardware.ps1') -Force

    $startnet = @'
@echo off
wpeinit
echo.
echo Windows Reinstaller - NON-DESTRUCTIVE HARDWARE TEST
echo No disks will be changed. The report is saved in X:\Logs.
if not exist X:\Logs md X:\Logs
powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File X:\Windows\System32\WindowsReinstaller\DetectHardware.ps1
echo.
echo Hardware detection is complete. This WinPE test will now stop.
echo Read X:\Logs\HardwareReport.txt above, then power off or close this window.
pause
'@
    Set-Content -LiteralPath (Join-Path $mountPath 'Windows\System32\startnet.cmd') -Value $startnet -Encoding Ascii

    Invoke-Dism @('/Unmount-Image', '/MountDir:' + $mountPath, '/Commit')
    $mounted = $false
    Write-Host "Non-destructive WinPE image built: $imagePath"
    Write-Host "The test only queries hardware and writes its report to the WinPE RAM drive (X:)."
    Write-Host "No USB writing, disk partitioning, formatting, or Windows installation is performed."
}
catch {
    if ($mounted) {
        & dism.exe /Unmount-Image /MountDir:$mountPath /Discard | Out-Host
        $mounted = $false
    }
    throw
}
