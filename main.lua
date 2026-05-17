local StateManager = require("src.core.state_manager")
local Game = require("src.core.game")

local stateManager = StateManager.new()

function love.load()
    love.graphics.setDefaultFilter("nearest", "nearest")
    stateManager:switch(Game.new())
end

function love.update(dt)
    stateManager:update(dt)
end

function love.draw()
    stateManager:draw()
end

function love.mousepressed(x, y, button)
    stateManager:mousepressed(x, y, button)
end

function love.mousereleased(x, y, button)
    stateManager:mousereleased(x, y, button)
end

function love.mousemoved(x, y, dx, dy)
    stateManager:mousemoved(x, y, dx, dy)
end
