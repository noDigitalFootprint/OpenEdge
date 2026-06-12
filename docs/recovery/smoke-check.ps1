param(
	[string]$ProjectPath = "src/OpenEdge/OpenEdge.csproj",
	[string]$RuntimeAppDir = "runtime/local/app",
	[int]$LaunchSeconds = 12,
	[switch]$SkipBuild
)

$ErrorActionPreference = "Stop"

function Assert-PathExists {
	param(
		[string]$TargetPath,
		[string]$Message
	)

	if (-not (Test-Path -LiteralPath $TargetPath)) {
		throw $Message
	}
}

$repoRoot = Split-Path -Parent $PSScriptRoot
$repoRoot = Split-Path -Parent $repoRoot

$resolvedProjectPath = Join-Path $repoRoot $ProjectPath
$resolvedRuntimeAppDir = Join-Path $repoRoot $RuntimeAppDir
$exePath = Join-Path $resolvedRuntimeAppDir "OpenEdge.exe"
$resourcesDir = Join-Path $resolvedRuntimeAppDir "resources"
$audioDir = Join-Path $resolvedRuntimeAppDir "audio"
$openFlagPath = Join-Path $resolvedRuntimeAppDir "flags\temp\open.txt"

Write-Host "[1/5] Checking repo paths..."
Assert-PathExists -TargetPath $resolvedProjectPath -Message "Project file not found: $resolvedProjectPath"

if (-not $SkipBuild) {
	Write-Host "[2/5] Building project..."
	& dotnet build $resolvedProjectPath
	if (-not $?) {
		throw "dotnet build failed"
	}
}
else {
	Write-Host "[2/5] Skipping build by request..."
}

Write-Host "[3/5] Verifying rebuilt output..."
Assert-PathExists -TargetPath $resolvedRuntimeAppDir -Message "Runtime app directory not found: $resolvedRuntimeAppDir"
Assert-PathExists -TargetPath $exePath -Message "Executable not found: $exePath"
Assert-PathExists -TargetPath $resourcesDir -Message "Resources directory not found: $resourcesDir"
Assert-PathExists -TargetPath $audioDir -Message "Audio directory not found: $audioDir"

$resourceCount = (Get-ChildItem -LiteralPath $resourcesDir -File | Measure-Object).Count
$audioCount = (Get-ChildItem -LiteralPath $audioDir -File | Measure-Object).Count

if ($resourceCount -lt 1) {
	throw "Resources directory is empty: $resourcesDir"
}

if ($audioCount -lt 1) {
	throw "Audio directory is empty: $audioDir"
}

$previousOpenWriteTimeUtc = $null
if (Test-Path -LiteralPath $openFlagPath) {
	$previousOpenWriteTimeUtc = (Get-Item -LiteralPath $openFlagPath).LastWriteTimeUtc
}

Write-Host "[4/5] Launching rebuilt app for $LaunchSeconds second(s)..."
$appProcess = Start-Process -FilePath $exePath -PassThru

try {
	Start-Sleep -Seconds $LaunchSeconds

	if ($appProcess.HasExited) {
		throw "OpenEdge exited early with code $($appProcess.ExitCode)"
	}

	Assert-PathExists -TargetPath $openFlagPath -Message "Startup flag was not written: $openFlagPath"
	$currentOpenWriteTimeUtc = (Get-Item -LiteralPath $openFlagPath).LastWriteTimeUtc

	if ($null -ne $previousOpenWriteTimeUtc -and $currentOpenWriteTimeUtc -lt $previousOpenWriteTimeUtc) {
		throw "Startup flag timestamp moved backwards unexpectedly"
	}
}
finally {
	if (-not $appProcess.HasExited) {
		Stop-Process -Id $appProcess.Id -Force
		Start-Sleep -Seconds 1
	}
}

Write-Host "[5/5] Smoke check passed."
Write-Host "- Executable: $exePath"
Write-Host "- Resources files: $resourceCount"
Write-Host "- Audio files: $audioCount"
Write-Host "- Startup flag: $openFlagPath"
