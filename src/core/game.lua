local Input = require("src.core.input")
local SaveManager = require("src.core.save_manager")
local StateManager = require("src.core.state_manager")
local TitleScene = require("src.scenes.title_scene")
local GameScene = require("src.scenes.game_scene")
local PauseScene = require("src.scenes.pause_scene")
local GameoverScene = require("src.scenes.gameover_scene")
local EndingScene = require("src.scenes.ending_scene")

local Game = {}
Game.__index = Game

function Game.new()
    return setmetatable({
        input = Input.new(),
        saveManager = SaveManager.new(),
        sceneManager = StateManager.new(),
        elapsed = 0,
        currentRun = nil
    }, Game)
end

function Game:enter()
    self.settings = self.saveManager:loadSettings()
    self.clearRecords = self.saveManager:loadClearRecords()
    self:showTitle()
end

function Game:update(dt)
    self.elapsed = self.elapsed + dt
    self.input:consumeClicks()
    self.sceneManager:update(dt)
end

function Game:draw()
    self.sceneManager:draw()
end

function Game:mousepressed(x, y, button)
    self.input:mousepressed(x, y, button)
    self.sceneManager:mousepressed(x, y, button)
end

function Game:mousereleased(_x, _y, _button)
end

function Game:mousemoved(x, y, dx, dy)
    self.sceneManager:mousemoved(x, y, dx, dy)
end

function Game:showTitle()
    self.currentRun = nil
    self.sceneManager:switch(TitleScene.new(self))
end

function Game:startStage1()
    self.currentRun = {
        currentRoom = "child_room",
        inventory = {}
    }
    self.sceneManager:switch(GameScene.new(self, self.currentRun))
end

function Game:pause()
    self.sceneManager:switch(PauseScene.new(self, self.sceneManager.current))
end

function Game:resume(scene)
    if scene then
        self.sceneManager:switch(scene)
    else
        self:startStage1()
    end
end

function Game:showGameOver()
    self.sceneManager:switch(GameoverScene.new(self))
end

function Game:restartFromChildRoom()
    self:startStage1()
end

function Game:showEnding()
    self.sceneManager:switch(EndingScene.new(self))
end

return Game
