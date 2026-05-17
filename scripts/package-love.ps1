Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
$buildDir = Join-Path $repoRoot 'build'
$output = Join-Path $buildDir 'EscapeFromNightmares.love'
$tempZip = Join-Path $buildDir 'EscapeFromNightmares.zip'

$phaseIndexPath = Join-Path $repoRoot 'phases\0-mvp\index.json'
if (-not (Test-Path -LiteralPath $phaseIndexPath -PathType Leaf)) {
    throw 'Missing phases/0-mvp/index.json; cannot verify resource fallback step.'
}

$phaseIndex = Get-Content -LiteralPath $phaseIndexPath -Raw | ConvertFrom-Json
$step12 = @($phaseIndex.steps | Where-Object { $_.step -eq 12 }) | Select-Object -First 1
if (-not $step12 -or $step12.status -ne 'completed') {
    throw 'Step 12 resource-fallback-assets is not completed; refusing to package .love.'
}

& (Join-Path $PSScriptRoot 'verify-resource-assets.ps1')
if ($LASTEXITCODE -ne 0) {
    throw 'Resource asset verification failed; refusing to package .love.'
}

New-Item -ItemType Directory -Force -Path $buildDir | Out-Null
if (Test-Path -LiteralPath $output) {
    Remove-Item -LiteralPath $output -Force
}
if (Test-Path -LiteralPath $tempZip) {
    Remove-Item -LiteralPath $tempZip -Force
}

$staging = Join-Path ([System.IO.Path]::GetTempPath()) ('efn-love-package-' + [System.Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $staging | Out-Null

try {
    $runtimeEntries = @('main.lua', 'conf.lua', 'src', 'data', 'assets')
    foreach ($entry in $runtimeEntries) {
        $source = Join-Path $repoRoot $entry
        if (-not (Test-Path -LiteralPath $source)) {
            throw "Missing required runtime entry: $entry"
        }
        Copy-Item -LiteralPath $source -Destination $staging -Recurse -Force
    }

    Compress-Archive -Path (Join-Path $staging '*') -DestinationPath $tempZip -Force
    Move-Item -LiteralPath $tempZip -Destination $output -Force
    Write-Host "Created $output"
}
finally {
    Remove-Item -LiteralPath $staging -Recurse -Force -ErrorAction SilentlyContinue
}
