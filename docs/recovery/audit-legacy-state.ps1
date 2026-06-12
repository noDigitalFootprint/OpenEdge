param(
    [switch]$IncludeMods,
    [string]$OutputPath
)

$ErrorActionPreference = 'Stop'
$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..\..')

$scriptRoots = @(
    Join-Path $repoRoot 'runtime\local\app\lines'
)
if ($IncludeMods) {
    $scriptRoots += Join-Path $repoRoot 'runtime\local\app\mods'
}

$sourceRoot = Join-Path $repoRoot 'src\OpenEdge'

$scriptPatterns = @(
    @{ Name = 'FLAG'; Pattern = '\bFLAG\s*:' },
    @{ Name = 'DELFLAG'; Pattern = '\bDELFLAG\s*:' },
    @{ Name = 'SETVAR'; Pattern = '\bSETVAR\s*:' },
    @{ Name = 'ADDVAR'; Pattern = '\bADDVAR\s*:' },
    @{ Name = 'ISFLAG'; Pattern = '\bISFLAG\s*:' },
    @{ Name = 'ISNOFLAG'; Pattern = '\bISNOFLAG\s*:' }
)
$codePatterns = @(
    @{ Name = 'getFlag'; Pattern = '\bgetFlag\s*\(' },
    @{ Name = 'setFlag'; Pattern = '\bsetFlag\s*\(' },
    @{ Name = 'getVar'; Pattern = '\bgetVar\s*\(' },
    @{ Name = 'setVar'; Pattern = '\bsetVar\s*\(' }
)

function Get-RelativePath {
    param([string]$Path)
    $rootPath = $repoRoot.ProviderPath
    if (-not $rootPath.EndsWith([System.IO.Path]::DirectorySeparatorChar)) {
        $rootPath += [System.IO.Path]::DirectorySeparatorChar
    }
    $rootUri = New-Object System.Uri($rootPath)
    $pathUri = New-Object System.Uri($Path)
    return [System.Uri]::UnescapeDataString($rootUri.MakeRelativeUri($pathUri).ToString()).Replace('\', '/')
}

function Find-Matches {
    param(
        [string[]]$Roots,
        [string[]]$Extensions,
        [object[]]$Patterns,
        [string[]]$ExcludeParts = @()
    )

    $items = New-Object System.Collections.Generic.List[object]
    foreach ($root in $Roots) {
        if (-not (Test-Path $root)) { continue }
        $files = Get-ChildItem -Path $root -Recurse -File | Where-Object {
            $Extensions -contains $_.Extension.ToLowerInvariant()
        }
        foreach ($part in $ExcludeParts) {
            $files = $files | Where-Object { $_.FullName -notmatch [Regex]::Escape("\$part\") }
        }
        foreach ($file in $files) {
            foreach ($entry in $Patterns) {
                $matches = Select-String -Path $file.FullName -Pattern $entry.Pattern -AllMatches -CaseSensitive:$false
                foreach ($match in $matches) {
                    $items.Add([pscustomobject]@{
                        Group = $entry.Name
                        File = Get-RelativePath $file.FullName
                        Line = $match.LineNumber
                        Text = $match.Line.Trim()
                    })
                }
            }
        }
    }
    return $items
}

$scriptMatches = Find-Matches -Roots $scriptRoots -Extensions @('.txt') -Patterns $scriptPatterns
$codeMatches = Find-Matches -Roots @($sourceRoot) -Extensions @('.cs') -Patterns $codePatterns -ExcludeParts @('bin','obj')
$allMatches = @($scriptMatches) + @($codeMatches)

$lines = New-Object System.Collections.Generic.List[string]
$lines.Add('# Legacy State Audit')
$lines.Add('')
$lines.Add("Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
$lines.Add("Include mods: $IncludeMods")
$lines.Add('')
$lines.Add('## Summary')
$lines.Add('')
if ($allMatches.Count -eq 0) {
    $lines.Add('- No legacy state usage found.')
}
else {
    $allMatches | Group-Object Group | Sort-Object Name | ForEach-Object {
        $lines.Add("- $($_.Name): $($_.Count)")
    }
}
$lines.Add('')
$lines.Add('## Matches')
$lines.Add('')
foreach ($group in ($allMatches | Group-Object Group | Sort-Object Name)) {
    $lines.Add("### $($group.Name)")
    $lines.Add('')
    foreach ($match in ($group.Group | Sort-Object File, Line, Text)) {
        $safeText = $match.Text.Replace('|', '\|')
        $lines.Add(('- `{0}:{1}` - {2}' -f $match.File, $match.Line, $safeText))
    }
    $lines.Add('')
}

$report = $lines -join [Environment]::NewLine
if ($OutputPath) {
    $resolvedOutput = if ([System.IO.Path]::IsPathRooted($OutputPath)) { $OutputPath } else { Join-Path $repoRoot $OutputPath }
    $parent = Split-Path -Parent $resolvedOutput
    if ($parent -and -not (Test-Path $parent)) { New-Item -ItemType Directory -Path $parent | Out-Null }
    Set-Content -Path $resolvedOutput -Value $report -Encoding UTF8
    Write-Host "Wrote audit report: $resolvedOutput"
}
else {
    Write-Output $report
}
