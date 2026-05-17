local SaveManager = {}
SaveManager.__index = SaveManager

local SETTINGS_PATH = "saves/settings.json"
local CLEAR_RECORDS_PATH = "saves/clear_records.json"

local DEFAULT_SETTINGS = {
    bgm_volume = 1.0,
    sfx_volume = 1.0
}

local DEFAULT_CLEAR_RECORDS = {
    stage1_clear = false
}

local function copyTable(source)
    local copied = {}
    for key, value in pairs(source) do
        copied[key] = value
    end
    return copied
end

local function ensureSaveDirectory()
    if love and love.filesystem then
        love.filesystem.createDirectory("saves")
    end
end

local function readFile(path)
    if not love or not love.filesystem or not love.filesystem.getInfo(path) then
        return nil
    end

    return love.filesystem.read(path)
end

local function writeFile(path, contents)
    if not love or not love.filesystem then
        return false
    end

    ensureSaveDirectory()
    return love.filesystem.write(path, contents)
end

local function encodeSettings(settings)
    return string.format(
        '{\n  "bgm_volume": %.2f,\n  "sfx_volume": %.2f\n}\n',
        settings.bgm_volume,
        settings.sfx_volume
    )
end

local function encodeClearRecords(records)
    return string.format(
        '{\n  "stage1_clear": %s\n}\n',
        records.stage1_clear and "true" or "false"
    )
end

local function readNumber(contents, key, fallback)
    local pattern = '"' .. key .. '"%s*:%s*([%d%.]+)'
    local value = contents and contents:match(pattern)
    return tonumber(value) or fallback
end

local function readBoolean(contents, key, fallback)
    local pattern = '"' .. key .. '"%s*:%s*(%a+)'
    local value = contents and contents:match(pattern)
    if value == "true" then
        return true
    end
    if value == "false" then
        return false
    end
    return fallback
end

function SaveManager.new()
    return setmetatable({}, SaveManager)
end

function SaveManager:loadSettings()
    local contents = readFile(SETTINGS_PATH)
    local settings = copyTable(DEFAULT_SETTINGS)

    settings.bgm_volume = readNumber(contents, "bgm_volume", settings.bgm_volume)
    settings.sfx_volume = readNumber(contents, "sfx_volume", settings.sfx_volume)

    return settings
end

function SaveManager:saveSettings(settings)
    local cleanSettings = {
        bgm_volume = tonumber(settings and settings.bgm_volume) or DEFAULT_SETTINGS.bgm_volume,
        sfx_volume = tonumber(settings and settings.sfx_volume) or DEFAULT_SETTINGS.sfx_volume
    }

    return writeFile(SETTINGS_PATH, encodeSettings(cleanSettings))
end

function SaveManager:loadClearRecords()
    local contents = readFile(CLEAR_RECORDS_PATH)
    local records = copyTable(DEFAULT_CLEAR_RECORDS)

    records.stage1_clear = readBoolean(contents, "stage1_clear", records.stage1_clear)

    return records
end

function SaveManager:saveClearRecords(records)
    local cleanRecords = {
        stage1_clear = records and records.stage1_clear == true
    }

    return writeFile(CLEAR_RECORDS_PATH, encodeClearRecords(cleanRecords))
end

function SaveManager:setStage1Clear()
    return self:saveClearRecords({
        stage1_clear = true
    })
end

return SaveManager
