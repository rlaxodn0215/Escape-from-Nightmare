Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
. (Join-Path $PSScriptRoot 'env.ps1')

$luac = Get-Command luac -ErrorAction SilentlyContinue
if ($luac) {
    $luaFiles = @(Get-ChildItem -LiteralPath $repoRoot -Recurse -Filter '*.lua' -File |
        Where-Object { $_.FullName -notmatch '\\build\\' })

    foreach ($file in $luaFiles) {
        & $luac.Source -p $file.FullName
        if ($LASTEXITCODE -ne 0) {
            exit $LASTEXITCODE
        }
    }

    Write-Host "Lua syntax OK via luac: $($luaFiles.Count) file(s)."
    exit 0
}

$loveCommand = Get-EfnLoveCommand
$luaFiles = @(Get-ChildItem -LiteralPath $repoRoot -Recurse -Filter '*.lua' -File |
    Where-Object { $_.FullName -notmatch '\\build\\' })

if ($luaFiles.Count -eq 0) {
    Write-Host 'Lua syntax OK: no Lua files found.'
    exit 0
}

$tempDir = Join-Path ([System.IO.Path]::GetTempPath()) ('efn-lua-syntax-' + [System.Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $tempDir | Out-Null

try {
    $checker = @'
local files = {}
for path in string.gmatch(os.getenv("EFN_SYNTAX_FILES") or "", "([^\n]+)") do
  files[#files + 1] = path
end

local failed = false
for _, path in ipairs(files) do
  local handle = io.open(path, "rb")
  if not handle then
    print("Cannot read Lua file: " .. path)
    failed = true
  else
    local source = handle:read("*a")
    handle:close()
    local chunk, err = loadstring(source, "@" .. path)
    if not chunk then
      print(err)
      failed = true
    end
  end
end

if failed then
  love.event.quit(1)
else
  print("Lua syntax OK via LOVE: " .. tostring(#files) .. " file(s).")
  love.event.quit(0)
end
'@

    Set-Content -LiteralPath (Join-Path $tempDir 'main.lua') -Value $checker -Encoding UTF8
    $env:EFN_SYNTAX_FILES = ($luaFiles.FullName -join "`n")
    & $loveCommand $tempDir
    exit $LASTEXITCODE
}
finally {
    Remove-Item -LiteralPath $tempDir -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item Env:\EFN_SYNTAX_FILES -ErrorAction SilentlyContinue
}
