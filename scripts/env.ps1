Set-StrictMode -Version Latest

function Get-EfnLoveCommand {
    $candidates = @()

    if ($env:EFN_LOVE_EXE) {
        $candidates += $env:EFN_LOVE_EXE
    }

    if ($env:LOVE_EXE) {
        $candidates += $env:LOVE_EXE
    }

    $lovec = Get-Command lovec -ErrorAction SilentlyContinue
    if ($lovec) {
        $candidates += $lovec.Source
    }

    $love = Get-Command love -ErrorAction SilentlyContinue
    if ($love) {
        $candidates += $love.Source
    }

    $candidates += @(
        'C:\Program Files\LOVE\lovec.exe',
        'C:\Program Files\LOVE\love.exe',
        'C:\Program Files (x86)\LOVE\lovec.exe',
        'C:\Program Files (x86)\LOVE\love.exe'
    )

    foreach ($candidate in $candidates) {
        if ($candidate -and (Test-Path -LiteralPath $candidate -PathType Leaf)) {
            return (Resolve-Path -LiteralPath $candidate).Path
        }
    }

    throw @'
LOVE executable was not found.
Set EFN_LOVE_EXE to a local love.exe or lovec.exe path, or install LOVE in C:\Program Files\LOVE.
'@
}
