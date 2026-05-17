local GameoverScene = {}
GameoverScene.__index = GameoverScene

local RESTART_BUTTON = { x = 520, y = 410, w = 240, h = 44, label = "Restart" }

local function contains(rect, x, y)
    return x >= rect.x and x <= rect.x + rect.w and y >= rect.y and y <= rect.y + rect.h
end

function GameoverScene.new(app)
    return setmetatable({
        app = app,
        timer = 0,
        restartDelay = 1.2
    }, GameoverScene)
end

function GameoverScene:enter()
    self.timer = 0
end

function GameoverScene:update(dt)
    self.timer = self.timer + dt
end

function GameoverScene:draw()
    love.graphics.clear(0.025, 0.005, 0.006, 1)

    love.graphics.setColor(0.72, 0.04, 0.04, 1)
    love.graphics.printf("Game Over", 0, 320, 1280, "center")

    if self.timer >= self.restartDelay then
        love.graphics.setColor(0.10, 0.10, 0.105, 1)
        love.graphics.rectangle("fill", RESTART_BUTTON.x, RESTART_BUTTON.y, RESTART_BUTTON.w, RESTART_BUTTON.h)
        love.graphics.setColor(0.45, 0.45, 0.45, 1)
        love.graphics.rectangle("line", RESTART_BUTTON.x, RESTART_BUTTON.y, RESTART_BUTTON.w, RESTART_BUTTON.h)
        love.graphics.setColor(0.82, 0.82, 0.82, 1)
        love.graphics.printf(RESTART_BUTTON.label, RESTART_BUTTON.x, RESTART_BUTTON.y + 13, RESTART_BUTTON.w, "center")
    end
end

function GameoverScene:mousepressed(x, y, button)
    if button ~= 1 or self.timer < self.restartDelay then
        return
    end

    if contains(RESTART_BUTTON, x, y) then
        self.app:restartFromChildRoom()
    end
end

return GameoverScene
