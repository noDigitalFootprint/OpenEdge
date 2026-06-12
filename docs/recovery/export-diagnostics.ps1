param(
    [string]$OutputPath = "docs/recovery/openedge-diagnostics.md"
)

$ErrorActionPreference = 'Stop'
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$resolvedOutput = if ([System.IO.Path]::IsPathRooted($OutputPath)) { $OutputPath } else { Join-Path $repoRoot $OutputPath }
$parent = Split-Path -Parent $resolvedOutput
if ($parent -and -not (Test-Path $parent)) { New-Item -ItemType Directory -Path $parent | Out-Null }

$runtimeRoot = Join-Path $repoRoot 'runtime\local\app'
$compatibilityState = Join-Path $runtimeRoot 'compatibility-state.json'
$mediaIndex = Join-Path $runtimeRoot 'media-tag-index.json'
$legacyTags = Join-Path $runtimeRoot 'tags.txt'
$auditTemp = Join-Path ([System.IO.Path]::GetTempPath()) ('openedge-audit-' + [Guid]::NewGuid().ToString('N') + '.md')

powershell -NoProfile -ExecutionPolicy Bypass -File (Join-Path $repoRoot 'docs\recovery\audit-legacy-state.ps1') -OutputPath $auditTemp | Out-Null
$audit = Get-Content $auditTemp -Raw
Remove-Item $auditTemp -ErrorAction SilentlyContinue

$compatibilityEntryCount = 0
if (Test-Path $compatibilityState) {
    $compatibilityText = Get-Content $compatibilityState -Raw
    $persistentMatch = [Regex]::Match($compatibilityText, '"PersistentEntries"\s*:\s*\{(?<body>[\s\S]*?)\}\s*[,}]')
    if ($persistentMatch.Success) {
        $compatibilityEntryCount = ([Regex]::Matches($persistentMatch.Groups['body'].Value, '"[^"]+"\s*:') | Measure-Object).Count
    }
}
$mediaIdentityCount = 0
$taggedIdentityCount = 0
if (Test-Path $mediaIndex) {
    $mediaJson = Get-Content $mediaIndex -Raw | ConvertFrom-Json
    if ($mediaJson.Items) {
        $mediaIdentityCount = @($mediaJson.Items).Count
        $taggedIdentityCount = @($mediaJson.Items | Where-Object { $_.Tags -and $_.Tags.Trim().Length -gt 0 }).Count
    }
}
$legacyTagLines = if (Test-Path $legacyTags) { @(Get-Content $legacyTags).Count } else { 0 }

$lines = New-Object System.Collections.Generic.List[string]
$lines.Add('# OpenEdge diagnostics export')
$lines.Add('')
$lines.Add("Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
$lines.Add('')
$lines.Add('## Compatibility state')
$lines.Add('')
$lines.Add("- Compatibility state file exists: $(Test-Path $compatibilityState)")
$lines.Add("- Persistent compatibility entries: $compatibilityEntryCount")
$lines.Add('')
$lines.Add('## Media tags')
$lines.Add('')
$lines.Add("- Media identity index exists: $(Test-Path $mediaIndex)")
$lines.Add("- Media identity records: $mediaIdentityCount")
$lines.Add("- Tagged identity records: $taggedIdentityCount")
$lines.Add("- Legacy tags.txt lines: $legacyTagLines")
$lines.Add('')
$lines.Add('## Script migration audit')
$lines.Add('')
$lines.Add($audit)

Set-Content -Path $resolvedOutput -Value ($lines -join [Environment]::NewLine) -Encoding UTF8
Write-Host "Wrote diagnostics report: $resolvedOutput"
