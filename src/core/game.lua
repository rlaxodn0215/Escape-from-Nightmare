local Input = require("src.core.input")
local SaveManager = require("src.core.save_manager")

local Game = {}
Game.__index = Game

function Game.new()
    return setmetatable({
        input = Input.new(),
        saveManager = SaveManager.new(),
        elapsed = 0
    }, Game)
end

function Game:enter()
    self.settings = self.saveManager:loadSettings()
    self.clearRecords = self.saveManager:loadClearRecords()
end

function Game:update(dt)
    self.elapsed = self.elapsed + dt
    self.input:consumeClicks()
end

function Game:draw()
    love.graphics.clear(0.02, 0.02, 0.025, 1)

    love.graphics.setColor(0.82, 0.82, 0.82, 1)
    love.graphics.printf("Escape From Nightmares", 0, 300, 1280, "center")

    love.graphics.setColor(0.42, 0.42, 0.42, 1)
    love.graphics.printf("Stage 1 skeleton", 0, 336, 1280, "center")
end

function Game:mousepressed(x, y, button)
    self.input:mousepressed(x, y, button)
end

function Game:mousereleased(_x, _y, _button)
end

return Game
