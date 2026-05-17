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
$imagePaths = @($referencedPaths | Where-Object { $_ -like 'assets/images/*' })
$soundPaths = @($referencedPaths | Where-Object { $_ -like 'assets/sounds/*' })

foreach ($path in $referencedPaths) {
    $fullPath = Join-Path $repoRoot $path
    $directory = Split-Path -Parent $fullPath
    if (-not (Test-Path -LiteralPath $directory -PathType Container)) {
        New-Item -ItemType Directory -Path $directory | Out-Null
    }
}

$missingImages = @($imagePaths | Where-Object { -not (Test-Path -LiteralPath (Join-Path $repoRoot $_) -PathType Leaf) })
$missingSounds = @($soundPaths | Where-Object { -not (Test-Path -LiteralPath (Join-Path $repoRoot $_) -PathType Leaf) })

if ($missingImages.Count -eq 0 -and $missingSounds.Count -eq 0) {
    Write-Host "No placeholder resources needed. Referenced assets already exist: $($referencedPaths.Count)."
    exit 0
}

$loveCommand = Get-EfnLoveCommand
$tempDir = Join-Path ([System.IO.Path]::GetTempPath()) ('efn-placeholder-assets-' + [System.Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $tempDir | Out-Null

try {
    $imageTable = ($missingImages | ForEach-Object { '  ' + (ConvertTo-LuaString $_) + ',' }) -join "`n"
    $soundTable = ''
    $repoLiteral = ConvertTo-LuaString $repoRoot.Path.Replace('\', '/')

    $generator = @"
local repoRoot = $repoLiteral
local images = {
$imageTable
}
local sounds = {
$soundTable
}

local function joinPath(root, relative)
  return root .. "/" .. relative
end

local function ensureParent(path)
  local normalized = path:gsub("\\", "/")
  local parent = normalized:match("^(.*)/[^/]+$")
  if parent then
    love.filesystem.createDirectory(parent)
  end
end

local function writeFile(path, bytes)
  ensureParent(path)
  local handle, err = io.open(path, "wb")
  if not handle then
    error("Cannot write placeholder asset " .. path .. ": " .. tostring(err))
  end
  handle:write(bytes)
  handle:close()
end

local function drawLabel(data, label)
  local text = label:gsub("^assets/images/", ""):gsub("%.png$", "")
  local width = data:getWidth()
  local height = data:getHeight()
  local y = math.max(10, math.floor(height * 0.52))
  for i = 1, #text do
    local code = text:byte(i)
    local x = 20 + ((i - 1) * 7)
    if x + 4 < width - 20 then
      for bit = 0, 4 do
        if math.floor(code / (2 ^ bit)) % 2 == 1 then
          for px = 0, 3 do
            for py = 0, 10 do
              local tx = x + bit * 5 + px
              local ty = y + py
              if tx >= 0 and tx < width and ty >= 0 and ty < height then
                data:setPixel(tx, ty, 0.34, 0.34, 0.34, 1)
              end
            end
          end
        end
      end
    end
  end
end

local function makeImage(relative)
  local isRoom = relative:find("^assets/images/rooms/") ~= nil
  local width = isRoom and 1280 or 128
  local height = isRoom and 720 or 128
  local data = love.image.newImageData(width, height)

  for y = 0, height - 1 do
    for x = 0, width - 1 do
      local shade = 0.035 + ((x + y) % 17) / 900
      data:setPixel(x, y, shade, shade, shade, 1)
    end
  end

  local border = isRoom and 4 or 2
  for y = 0, height - 1 do
    for x = 0, width - 1 do
      if x < border or y < border or x >= width - border or y >= height - border then
        data:setPixel(x, y, 0.18, 0.18, 0.18, 1)
      end
    end
  end

  if isRoom then
    for x = 0, width - 1 do
      local y = math.floor(height * 0.66 + math.sin(x / 37) * 8)
      if y >= 0 and y < height then
        data:setPixel(x, y, 0.12, 0.12, 0.12, 1)
      end
    end
  end

  drawLabel(data, relative)
  local fileData = data:encode("png")
  writeFile(joinPath(repoRoot, relative), fileData:getString())
end

local function makeSound(relative)
  local sampleRate = 22050
  local duration = 0.25
  local soundData = love.sound.newSoundData(math.floor(sampleRate * duration), sampleRate, 16, 1)
  for i = 0, soundData:getSampleCount() - 1 do
    soundData:setSample(i, 0)
  end
  local fileData = soundData:encode("ogg")
  writeFile(joinPath(repoRoot, relative), fileData:getString())
end

function love.load()
  local ok, err = pcall(function()
    for _, relative in ipairs(images) do
      makeImage(relative)
    end
    for _, relative in ipairs(sounds) do
      makeSound(relative)
    end
  end)

  if not ok then
    print(err)
    love.event.quit(1)
  else
    print("Generated placeholder images: " .. tostring(#images))
    print("Generated placeholder sounds: " .. tostring(#sounds))
    love.event.quit(0)
  end
end
"@

    Set-Content -LiteralPath (Join-Path $tempDir 'main.lua') -Value $generator -Encoding UTF8
    & $loveCommand $tempDir
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }

    if ($missingSounds.Count -gt 0) {
        $ffmpeg = Get-FfmpegCommand
        foreach ($relative in $missingSounds) {
            $target = Join-Path $repoRoot $relative
            & $ffmpeg -hide_banner -loglevel error -f lavfi -i anullsrc=r=22050:cl=mono -t 0.25 -c:a libvorbis -q:a 0 -y $target
            if ($LASTEXITCODE -ne 0) {
                throw "Failed to generate silent OGG placeholder: $relative"
            }
        }
        Write-Host "Generated placeholder sounds: $($missingSounds.Count)"
    }
}
finally {
    Remove-Item -LiteralPath $tempDir -Recurse -Force -ErrorAction SilentlyContinue
}
