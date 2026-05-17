local GameScene = {}
GameScene.__index = GameScene

local RoomSystem = require("src.systems.room_system")
local InteractionSystem = require("src.systems.interaction_system")
local Rooms = require("data.rooms")
local RoomObjects = require("data.room_objects")

local BUTTONS = {
    inventory = { x = 24, y = 24, w = 112, h = 36, label = "Inventory" },
    map = { x = 148, y = 24, w = 76, h = 36, label = "Map" },
    pause = { x = 1148, y = 24, w = 108, h = 36, label = "Pause" }
}

local function contains(rect, x, y)
    return x >= rect.x and x <= rect.x + rect.w and y >= rect.y and y <= rect.y + rect.h
end

local function drawButton(button)
    love.graphics.setColor(0.08, 0.08, 0.085, 0.92)
    love.graphics.rectangle("fill", button.x, button.y, button.w, button.h)
    love.graphics.setColor(0.38, 0.38, 0.38, 1)
    love.graphics.rectangle("line", button.x, button.y, button.w, button.h)
    love.graphics.setColor(0.76, 0.76, 0.76, 1)
    love.graphics.printf(button.label, button.x, button.y + 10, button.w, "center")
end

function GameScene.new(app, run)
    return setmetatable({
        app = app,
        run = run,
        roomSystem = nil,
        interactionSystem = nil,
        notice = nil
    }, GameScene)
end

function GameScene:enter()
    self.roomSystem = RoomSystem.new(Rooms, self.run.currentRoom or Rooms.startRoom)
    self.interactionSystem = InteractionSystem.new(RoomObjects)
    self.run.currentRoom = self.roomSystem:getCurrentRoomId()
end

function GameScene:update(_dt)
end

function GameScene:draw()
    local room = self.roomSystem and self.roomSystem:getCurrentRoom() or nil
    local currentRoomId = self.roomSystem and self.roomSystem:getCurrentRoomId() or self.run.currentRoom

    love.graphics.clear(0.018, 0.018, 0.022, 1)

    love.graphics.setColor(0.09, 0.09, 0.095, 1)
    love.graphics.rectangle("fill", 0, 86, 1280, 548)
    love.graphics.setColor(0.22, 0.22, 0.22, 1)
    love.graphics.rectangle("line", 76, 128, 1128, 448)

    love.graphics.setColor(0.36, 0.36, 0.36, 1)
    love.graphics.printf(currentRoomId, 0, 332, 1280, "center")

    if room then
        love.graphics.setColor(0.22, 0.22, 0.22, 1)
        love.graphics.printf(room.background, 0, 366, 1280, "center")
    end

    drawButton(BUTTONS.inventory)
    drawButton(BUTTONS.map)
    drawButton(BUTTONS.pause)

    if self.notice then
        love.graphics.setColor(0.62, 0.62, 0.62, 1)
        love.graphics.printf(self.notice, 0, 646, 1280, "center")
    end
end

function GameScene:mousepressed(x, y, button)
    if button ~= 1 then
        return
    end

    if contains(BUTTONS.pause, x, y) then
        self.app:pause()
    elseif contains(BUTTONS.inventory, x, y) then
        self.notice = "Inventory shell"
    elseif contains(BUTTONS.map, x, y) then
        self.notice = "Map shell"
    elseif self.interactionSystem and self.roomSystem then
        local result = self.interactionSystem:handleClick(self.roomSystem, x, y)

        if result.moved then
            self.run.currentRoom = result.roomId
            self.notice = nil
        elseif result.blocked and result.reason == "locked" then
            self.notice = "Locked"
        elseif result.blocked and result.reason == "escape_locked" then
            self.notice = "Locked"
        end
    end
end

return GameScene
