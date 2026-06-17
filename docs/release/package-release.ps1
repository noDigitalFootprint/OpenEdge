param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [string]$Configuration = "Release"
)

$ErrorActionPreference = 'Stop'

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$projectPath = Join-Path $repoRoot 'src\OpenEdge\OpenEdge.csproj'
$artifactsRoot = Join-Path $repoRoot 'artifacts'
$publishRoot = Join-Path $artifactsRoot 'publish\OpenEdge'
$stageRoot = Join-Path $artifactsRoot 'release\OpenEdge'
$zipPath = Join-Path $artifactsRoot ("OpenEdge-$Version-win-x86.zip")

$forbiddenRelativePaths = @(
    'options.txt',
    'tasks.txt',
    'tagGroups.txt',
    'tags.txt',
    'compatibility-state.json',
    'media-sources.json',
    'media-sources.json.backup',
    'media-tag-index.json',
    'media-tag-index.json.backup',
    'ICAddress.txt',
    'ICPath.txt',
    'btDevices.txt',
    'flags',
    'debug',
    'images',
    'videos',
    'mods',
    'contexts',
    'compatibility-backups',
    'compatibility-transfers'
)

function Remove-IfExists([string]$Path) {
    if (Test-Path $Path) {
        Remove-Item -Path $Path -Recurse -Force
    }
}

function Assert-NoForbiddenPayload([string]$Root) {
    $found = New-Object System.Collections.Generic.List[string]
    foreach ($relativePath in $forbiddenRelativePaths) {
        $candidate = Join-Path $Root $relativePath
        if (Test-Path $candidate) {
            $found.Add($relativePath)
        }
    }
    if ($found.Count -gt 0) {
        throw "Release payload contains user-state paths:`n - $($found -join "`n - ")"
    }
}

function Copy-IfExists([string]$Source, [string]$Destination) {
    if (Test-Path $Source) {
        Copy-Item -Path $Source -Destination $Destination -Recurse -Force
    }
}

Write-Host "Packaging OpenEdge $Version ($Configuration)..."
Write-Host "Repository: $repoRoot"

Remove-IfExists $publishRoot
Remove-IfExists $stageRoot
if (Test-Path $zipPath) {
    Remove-Item -Path $zipPath -Force
}
New-Item -ItemType Directory -Path $publishRoot | Out-Null
New-Item -ItemType Directory -Path $stageRoot | Out-Null

Write-Host "Publishing project..."
dotnet publish $projectPath -c $Configuration -o $publishRoot

Write-Host "Copying release-safe payload..."
Get-ChildItem -Path $publishRoot -File | Where-Object {
    $_.Extension -in @('.exe', '.dll', '.json', '.config', '.pdb') -or
    $_.Name -like '*.deps.json' -or
    $_.Name -like '*.runtimeconfig.json'
} | ForEach-Object {
    Copy-Item -Path $_.FullName -Destination (Join-Path $stageRoot $_.Name) -Force
}

Copy-IfExists (Join-Path $publishRoot 'resources') (Join-Path $stageRoot 'resources')
Copy-IfExists (Join-Path $publishRoot 'audio') (Join-Path $stageRoot 'audio')
Copy-IfExists (Join-Path $publishRoot 'lines') (Join-Path $stageRoot 'lines')

if (-not (Test-Path (Join-Path $stageRoot 'OpenEdge.exe'))) {
    throw 'Release payload is missing OpenEdge.exe.'
}

Assert-NoForbiddenPayload $stageRoot

Write-Host "Creating zip..."
Compress-Archive -Path (Join-Path $stageRoot '*') -DestinationPath $zipPath -Force

Write-Host "Release package created: $zipPath"
Write-Host "Contents are staged at: $stageRoot"
Write-Host "User data is intentionally excluded. Users can extract this zip over an existing OpenEdge folder to update."
