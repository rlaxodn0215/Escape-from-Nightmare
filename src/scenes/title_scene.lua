local TitleScene = {}
TitleScene.__index = TitleScene

local BUTTONS = {
    start = { x = 540, y = 332, w = 200, h = 44, label = "Start" },
    settings = { x = 540, y = 388, w = 200, h = 44, label = "Settings" },
    quit = { x = 540, y = 444, w = 200, h = 44, label = "Quit" }
}

local function contains(rect, x, y)
    return x >= rect.x and x <= rect.x + rect.w and y >= rect.y and y <= rect.y + rect.h
end

local function drawButton(button)
    love.graphics.setColor(0.12, 0.12, 0.13, 1)
    love.graphics.rectangle("fill", button.x, button.y, button.w, button.h)
    love.graphics.setColor(0.48, 0.48, 0.48, 1)
    love.graphics.rectangle("line", button.x, button.y, button.w, button.h)
    love.graphics.setColor(0.84, 0.84, 0.84, 1)
    love.graphics.printf(button.label, button.x, button.y + 13, button.w, "center")
end

function TitleScene.new(app)
    return setmetatable({
        app = app,
        settingsOpen = false
    }, TitleScene)
end

function TitleScene:enter()
end

function TitleScene:update(_dt)
end

function TitleScene:draw()
    love.graphics.clear(0.015, 0.015, 0.018, 1)

    love.graphics.setColor(0.86, 0.86, 0.86, 1)
    love.graphics.printf("Escape From Nightmares", 0, 238, 1280, "center")

    love.graphics.setColor(0.35, 0.35, 0.35, 1)
    love.graphics.printf("Stage 1", 0, 274, 1280, "center")

    drawButton(BUTTONS.start)
    drawButton(BUTTONS.settings)
    drawButton(BUTTONS.quit)

    if self.settingsOpen then
        love.graphics.setColor(0.05, 0.05, 0.055, 0.96)
        love.graphics.rectangle("fill", 430, 542, 420, 76)
        love.graphics.setColor(0.45, 0.45, 0.45, 1)
        love.graphics.rectangle("line", 430, 542, 420, 76)
        love.graphics.setColor(0.78, 0.78, 0.78, 1)
        love.graphics.printf("Settings shell: BGM / SFX volume", 430, 572, 420, "center")
    end
end

function TitleScene:mousepressed(x, y, button)
    if button ~= 1 then
        return
    end

    if contains(BUTTONS.start, x, y) then
        self.app:startStage1()
    elseif contains(BUTTONS.settings, x, y) then
        self.settingsOpen = not self.settingsOpen
    elseif contains(BUTTONS.quit, x, y) then
        love.event.quit()
    end
end

return TitleScene
