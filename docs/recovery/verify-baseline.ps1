param(
    [switch]$SkipSmoke,
    [switch]$SkipAudit
)

$ErrorActionPreference = 'Stop'
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$project = Join-Path $repoRoot 'src\OpenEdge\OpenEdge.csproj'
$harness = Join-Path $repoRoot 'tests\SettingsHarness\SettingsHarness.csproj'
$smoke = Join-Path $repoRoot 'docs\recovery\smoke-check.ps1'
$audit = Join-Path $repoRoot 'docs\recovery\audit-legacy-state.ps1'
$auditOutput = Join-Path $repoRoot 'docs\recovery\legacy-state-audit.md'

function Invoke-Step {
    param(
        [string]$Name,
        [scriptblock]$Action
    )

    Write-Host ""
    Write-Host "== $Name =="
    & $Action
    if ($LASTEXITCODE -ne $null -and $LASTEXITCODE -ne 0) {
        throw "$Name failed with exit code $LASTEXITCODE"
    }
    Write-Host "PASS: $Name"
}

Push-Location $repoRoot
try {
    Invoke-Step 'Build OpenEdge' { dotnet build $project }
    Invoke-Step 'Run SettingsHarness' { dotnet run --project $harness }
    if (-not $SkipAudit) {
        Invoke-Step 'Run legacy-state audit' { powershell -NoProfile -ExecutionPolicy Bypass -File $audit -OutputPath $auditOutput }
    }
    else {
        Write-Host ""
        Write-Host 'SKIP: Run legacy-state audit (-SkipAudit supplied)'
    }
    if (-not $SkipSmoke) {
        Invoke-Step 'Run smoke check' { powershell -ExecutionPolicy Bypass -File $smoke }
    }
    else {
        Write-Host ""
        Write-Host 'SKIP: Run smoke check (-SkipSmoke supplied)'
    }

    Write-Host ""
    Write-Host 'Verification passed.'
}
finally {
    Pop-Location
}
