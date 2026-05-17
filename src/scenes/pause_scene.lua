local PauseScene = {}
PauseScene.__index = PauseScene

local BUTTONS = {
    continue = { x = 520, y = 294, w = 240, h = 44, label = "Continue" },
    settings = { x = 520, y = 350, w = 240, h = 44, label = "Settings" },
    title = { x = 520, y = 406, w = 240, h = 44, label = "Return to Title" }
}

local function contains(rect, x, y)
    return x >= rect.x and x <= rect.x + rect.w and y >= rect.y and y <= rect.y + rect.h
end

local function drawButton(button)
    love.graphics.setColor(0.10, 0.10, 0.105, 1)
    love.graphics.rectangle("fill", button.x, button.y, button.w, button.h)
    love.graphics.setColor(0.45, 0.45, 0.45, 1)
    love.graphics.rectangle("line", button.x, button.y, button.w, button.h)
    love.graphics.setColor(0.82, 0.82, 0.82, 1)
    love.graphics.printf(button.label, button.x, button.y + 13, button.w, "center")
end

function PauseScene.new(app, previousScene)
    return setmetatable({
        app = app,
        previousScene = previousScene,
        settingsOpen = false
    }, PauseScene)
end

function PauseScene:enter()
end

function PauseScene:update(_dt)
end

function PauseScene:draw()
    love.graphics.clear(0.012, 0.012, 0.015, 1)

    love.graphics.setColor(0.82, 0.82, 0.82, 1)
    love.graphics.printf("Paused", 0, 236, 1280, "center")

    drawButton(BUTTONS.continue)
    drawButton(BUTTONS.settings)
    drawButton(BUTTONS.title)

    if self.settingsOpen then
        love.graphics.setColor(0.46, 0.46, 0.46, 1)
        love.graphics.printf("Settings shell: BGM / SFX volume", 0, 482, 1280, "center")
    end
end

function PauseScene:mousepressed(x, y, button)
    if button ~= 1 then
        return
    end

    if contains(BUTTONS.continue, x, y) then
        self.app:resume(self.previousScene)
    elseif contains(BUTTONS.settings, x, y) then
        self.settingsOpen = not self.settingsOpen
    elseif contains(BUTTONS.title, x, y) then
        self.app:showTitle()
    end
end

return PauseScene
