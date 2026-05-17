Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
$buildDir = Join-Path $repoRoot 'build'
$output = Join-Path $buildDir 'EscapeFromNightmares.love'

New-Item -ItemType Directory -Force -Path $buildDir | Out-Null
if (Test-Path -LiteralPath $output) {
    Remove-Item -LiteralPath $output -Force
}

$staging = Join-Path ([System.IO.Path]::GetTempPath()) ('efn-love-package-' + [System.Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $staging | Out-Null

try {
    Get-ChildItem -LiteralPath $repoRoot -Force |
        Where-Object { $_.Name -notin @('.git', 'build', 'saves') } |
        ForEach-Object {
            Copy-Item -LiteralPath $_.FullName -Destination $staging -Recurse -Force
        }

    Compress-Archive -Path (Join-Path $staging '*') -DestinationPath $output -Force
    Write-Host "Created $output"
}
finally {
    Remove-Item -LiteralPath $staging -Recurse -Force -ErrorAction SilentlyContinue
}
