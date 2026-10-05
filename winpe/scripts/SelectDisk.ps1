param(
    [switch]$WhatIf
)

Write-Host "Disk selection placeholder."

if ($WhatIf) {
    Write-Host "WHATIF: no changes would be made."
}

# Production implementation should require an explicit, validated disk identity.
