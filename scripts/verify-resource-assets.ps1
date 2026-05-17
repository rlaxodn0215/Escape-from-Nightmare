Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
. (Join-Path $PSScriptRoot 'env.ps1')

function Get-ReferencedAssetPaths {
    $dataFiles = @(Get-ChildItem -LiteralPath (Join-Path $repoRoot 'data') -Recurse -Filter '*.lua' -File)
    $paths = New-Object System.Collections.Generic.HashSet[string]

    foreach ($file in $dataFiles) {
        $text = Get-Content -LiteralPath $file.FullName -Raw
        $matches = [regex]::Matches($text, 'assets/(?:images|sounds)/[A-Za-z0-9_\-./]+')
        foreach ($match in $matches) {
            [void]$paths.Add($match.Value.Replace('\', '/'))
        }
    }

    return @($paths | Sort-Object)
}

function ConvertTo-LuaString {
    param([Parameter(Mandatory = $true)][string]$Value)
    return '"' + ($Value -replace '\\', '\\' -replace '"', '\"') + '"'
}

function Get-FfmpegCommand {
    if ($env:EFN_FFMPEG -and (Test-Path -LiteralPath $env:EFN_FFMPEG -PathType Leaf)) {
        return $env:EFN_FFMPEG
    }

    $command = Get-Command ffmpeg -ErrorAction SilentlyContinue
    if ($command) {
        return $command.Source
    }

    $imageioRoot = Join-Path $HOME '.cache\codex-runtimes\codex-primary-runtime\dependencies\python\Lib\site-packages\imageio_ffmpeg\binaries'
    if (Test-Path -LiteralPath $imageioRoot -PathType Container) {
        $imageioFfmpeg = Get-ChildItem -LiteralPath $imageioRoot -Filter 'ffmpeg*.exe' -File | Select-Object -First 1
        if ($imageioFfmpeg) {
            return $imageioFfmpeg.FullName
        }
    }

    throw 'No ffmpeg executable found. Set EFN_FFMPEG, put ffmpeg on PATH, or install approved imageio-ffmpeg.'
}

$referencedPaths = @(Get-ReferencedAssetPaths)
if ($referencedPaths.Count -eq 0) {
    throw 'No assets/images or assets/sounds paths were found in data/*.lua.'
}

$assetsFiles = @(Get-ChildItem -LiteralPath (Join-Path $repoRoot 'assets') -Recurse -File)
if ($assetsFiles.Count -eq 0) {
    throw 'assets/ contains zero files.'
}

$missing = @()
foreach ($path in $referencedPaths) {
    if (-not (Test-Path -LiteralPath (Join-Path $repoRoot $path) -PathType Leaf)) {
        $missing += $path
    }
}

if ($missing.Count -gt 0) {
    throw "Missing referenced asset file(s):`n$($missing -join "`n")"
}

$imagePaths = @($referencedPaths | Where-Object { $_ -like 'assets/images/*' })
$soundPaths = @($referencedPaths | Where-Object { $_ -like 'assets/sounds/*' })
[void][System.Reflection.Assembly]::LoadWithPartialName('System.Drawing')

foreach ($relative in $imagePaths) {
    $fullPath = Join-Path $repoRoot $relative
    $bitmap = $null
    try {
        $bitmap = [System.Drawing.Bitmap]::new($fullPath)
        if ($bitmap.Width -le 0 -or $bitmap.Height -le 0) {
            throw "Image has invalid dimensions: $relative"
        }
        if ($relative -like 'assets/images/rooms/*' -and ($bitmap.Width -ne 1280 -or $bitmap.Height -ne 720)) {
            throw "Room background must be 1280x720, got $($bitmap.Width)x$($bitmap.Height): $relative"
        }
    }
    finally {
        if ($bitmap) {
            $bitmap.Dispose()
        }
    }
}

if ($soundPaths.Count -gt 0) {
    $ffmpeg = Get-FfmpegCommand
    foreach ($relative in $soundPaths) {
        $fullPath = Join-Path $repoRoot $relative
        & $ffmpeg -hide_banner -loglevel error -i $fullPath -f null -
        if ($LASTEXITCODE -ne 0) {
            throw "Sound is not readable by ffmpeg: $relative"
        }
    }
}

Write-Host "Resource assets OK: $($imagePaths.Count) image(s), $($soundPaths.Count) sound(s)."
Write-Host "Referenced assets exist and are readable: $($referencedPaths.Count)."
