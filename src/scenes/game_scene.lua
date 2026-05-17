local GameScene = {}
GameScene.__index = GameScene

local RoomSystem = require("src.systems.room_system")
local InteractionSystem = require("src.systems.interaction_system")
local InventorySystem = require("src.systems.inventory_system")
local InventoryUi = require("src.ui.inventory_ui")
local Rooms = require("data.rooms")
local RoomObjects = require("data.room_objects")
local Items = require("data.items")

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
        inventorySystem = nil,
        inventoryUi = nil,
        notice = nil
    }, GameScene)
end

function GameScene:enter()
    self.roomSystem = RoomSystem.new(Rooms, self.run.currentRoom or Rooms.startRoom)
    self.interactionSystem = InteractionSystem.new(RoomObjects)
    self.inventorySystem = InventorySystem.new(Items, self.run.inventory)
    self.inventoryUi = InventoryUi.new(self.inventorySystem)
    self.run.inventory = self.inventorySystem.state
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

    local selectedItem = self.inventorySystem and self.inventorySystem:getSelectedItem() or nil
    if selectedItem then
        love.graphics.setColor(0.52, 0.08, 0.08, 1)
        love.graphics.rectangle("line", 24, 642, 256, 38)
        love.graphics.setColor(0.74, 0.74, 0.74, 1)
        love.graphics.printf("Using: " .. selectedItem.name, 34, 654, 236, "left")
    end

    if self.notice then
        love.graphics.setColor(0.62, 0.62, 0.62, 1)
        love.graphics.printf(self.notice, 0, 646, 1280, "center")
    end

    if self.inventoryUi then
        self.inventoryUi:draw()
    end
end

function GameScene:mousepressed(x, y, button)
    if button ~= 1 then
        return
    end

    if self.inventoryUi and self.inventoryUi:isOpen() then
        local uiResult = self.inventoryUi:handleClick(x, y)
        if uiResult.selectedItem then
            self.notice = "Select a target"
        end
        if uiResult.handled then
            return
        end
    end

    if contains(BUTTONS.pause, x, y) then
        self.app:pause()
    elseif contains(BUTTONS.inventory, x, y) then
        self.inventoryUi:toggle()
        self.notice = nil
    elseif contains(BUTTONS.map, x, y) then
        self.notice = "Map shell"
    elseif self.interactionSystem and self.roomSystem then
        local result = self.interactionSystem:handleClick(self.roomSystem, x, y, self.inventorySystem)

        if result.moved then
            self.run.currentRoom = result.roomId
            self.notice = nil
        elseif result.itemPickup then
            self.notice = "Taken: " .. result.item.name
        elseif result.reason == "already_acquired" then
            self.notice = "Already taken"
        elseif result.itemUsed then
            self.notice = "Used: " .. result.item.name
        elseif result.blocked and result.reason == "wrong_target" then
            self.notice = "No reaction"
        elseif result.blocked and result.reason == "locked" then
            self.notice = "Locked"
        elseif result.blocked and result.reason == "escape_locked" then
            self.notice = "Locked"
        end
    end
end

return GameScene
