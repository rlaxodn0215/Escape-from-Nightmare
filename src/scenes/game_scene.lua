local GameScene = {}
GameScene.__index = GameScene

local RoomSystem = require("src.systems.room_system")
local InteractionSystem = require("src.systems.interaction_system")
local InventorySystem = require("src.systems.inventory_system")
local MapSystem = require("src.systems.map_system")
local PuzzleSystem = require("src.systems.puzzle_system")
local HidingSystem = require("src.systems.hiding_system")
local InventoryUi = require("src.ui.inventory_ui")
local MapUi = require("src.ui.map_ui")
local PuzzleUi = require("src.ui.puzzle_ui")
local DangerGaugeUi = require("src.ui.danger_gauge_ui")
local Monster = require("src.ai.monster")
local Danger = require("src.ai.danger")
local Rooms = require("data.rooms")
local RoomObjects = require("data.room_objects")
local Items = require("data.items")
local PuzzleInputs = require("data.puzzle_inputs")
local Events = require("data.events")
local MonsterNodes = require("data.monster_nodes")

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
        mapSystem = nil,
        puzzleSystem = nil,
        hidingSystem = nil,
        dangerSystem = nil,
        monster = nil,
        inventoryUi = nil,
        mapUi = nil,
        puzzleUi = nil,
        dangerGaugeUi = nil,
        notice = nil
    }, GameScene)
end

function GameScene:enter()
    self.run.flags = self.run.flags or {}
    self.roomSystem = RoomSystem.new(Rooms, self.run.currentRoom or Rooms.startRoom)
    self.interactionSystem = InteractionSystem.new(RoomObjects, self.run.flags)
    self.inventorySystem = InventorySystem.new(Items, self.run.inventory)
    self.mapSystem = MapSystem.new(self.roomSystem)
    self.dangerSystem = Danger.new(self.run, Events)
    self.puzzleSystem = PuzzleSystem.new(PuzzleInputs, Events, self.inventorySystem, self.run, self.dangerSystem)
    self.monster = Monster.new(MonsterNodes, Events, self.run)
    self.hidingSystem = HidingSystem.new(self.run, self.dangerSystem, self.monster)
    self.inventoryUi = InventoryUi.new(self.inventorySystem)
    self.mapUi = MapUi.new(self.mapSystem)
    self.puzzleUi = PuzzleUi.new(self.puzzleSystem)
    self.dangerGaugeUi = DangerGaugeUi.new(self.hidingSystem)
    self.run.inventory = self.inventorySystem.state
    self.run.currentRoom = self.roomSystem:getCurrentRoomId()
end

function GameScene:update(dt)
    if self.monster then
        self.monster:update(dt)
    end

    if self.dangerSystem then
        self.dangerSystem:update(dt, self.monster and self.monster:getState() or nil)

        if self.dangerSystem:isCaptureFull() then
            if self.monster then
                self.monster:triggerEvent("event_player_captured", self.roomSystem:getCurrentRoomId())
            end
            self.app:showGameOver()
            return
        end
    end

    if self.hidingSystem then
        self.hidingSystem:update(dt, self.roomSystem:getCurrentRoomId())

        if self.hidingSystem:getState().detected then
            self.app:showGameOver()
            return
        end
    end

    if self.puzzleUi then
        self.puzzleUi:update(dt)
    end
end

function GameScene:draw()
    local room = self.roomSystem and self.roomSystem:getCurrentRoom() or nil
    love.graphics.clear(0.018, 0.018, 0.022, 1)

    love.graphics.setColor(0.09, 0.09, 0.095, 1)
    love.graphics.rectangle("fill", 0, 86, 1280, 548)
    love.graphics.setColor(0.22, 0.22, 0.22, 1)
    love.graphics.rectangle("line", 76, 128, 1128, 448)

    if room then
        love.graphics.setColor(0.22, 0.22, 0.22, 1)
        love.graphics.printf("Room view placeholder", 0, 366, 1280, "center")
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

    if self.mapUi then
        self.mapUi:draw()
    end

    if self.puzzleUi then
        self.puzzleUi:draw()
    end

    if self.dangerGaugeUi then
        self.dangerGaugeUi:draw()
    end
end

function GameScene:describePuzzleResult(result)
    if result.alreadySolved then
        return "Already solved"
    end

    if result.solved then
        if result.clearFlag == "stage1_clear" then
            return "Unlocked"
        end

        local names = {}
        for _, item in ipairs(result.addedItems or {}) do
            names[#names + 1] = item.name
        end

        if #names > 0 then
            return "Taken: " .. table.concat(names, ", ")
        end

        if result.spawnedObjects and #result.spawnedObjects > 0 then
            return "Something changed"
        end

        return "Solved"
    end

    if result.failed then
        return "No reaction"
    end

    return nil
end

function GameScene:handlePuzzleResult(result)
    self.notice = self:describePuzzleResult(result)

    if result.solved and result.clearFlag == "stage1_clear" then
        if self.app.saveManager then
            self.app.saveManager:setStage1Clear()
        end
        self.app:showEnding()
    end
end

function GameScene:mousepressed(x, y, button)
    if button ~= 1 then
        return
    end

    if self.hidingSystem and self.hidingSystem:isActive() then
        local result = self.hidingSystem:exit()
        if result.exited then
            self.notice = result.finalChaseHideSuccess and "It passed" or nil
        elseif result.reason == "too_early" then
            self.notice = "Too close"
        end
        return
    end

    if self.puzzleUi and self.puzzleUi:isOpen() then
        local uiResult = self.puzzleUi:handleClick(x, y)
        if uiResult.result then
            self:handlePuzzleResult(uiResult.result)
        end
        if uiResult.handled then
            return
        end
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

    if self.mapUi and self.mapUi:isOpen() then
        local uiResult = self.mapUi:handleClick(x, y)
        if uiResult.handled then
            return
        end
    end

    if contains(BUTTONS.pause, x, y) then
        self.app:pause()
    elseif contains(BUTTONS.inventory, x, y) then
        self.inventoryUi:toggle()
        if self.mapUi then
            self.mapUi:close()
        end
        self.notice = nil
    elseif contains(BUTTONS.map, x, y) then
        self.mapUi:toggle()
        if self.inventoryUi then
            self.inventoryUi:close()
        end
        self.notice = nil
    elseif self.interactionSystem and self.roomSystem then
        local result = self.interactionSystem:handleClick(self.roomSystem, x, y, self.inventorySystem)

        if result.hiding then
            self.hidingSystem:enter(self.roomSystem:getCurrentRoomId(), result.object)
            if self.inventoryUi then
                self.inventoryUi:close()
            end
            if self.mapUi then
                self.mapUi:close()
            end
            self.notice = nil
        elseif result.moved then
            self.run.currentRoom = result.roomId
            if self.monster then
                self.monster:setPlayerRoom(result.roomId)
                if result.roomId == "kitchen" and not self.run.events.event_kitchen_first_appearance then
                    self.monster:triggerEvent("event_kitchen_first_appearance", result.roomId)
                end
            end
            if self.dangerSystem then
                self.dangerSystem:setPlayerRoom(result.roomId)
            end
            self.notice = nil
        elseif result.itemPickup then
            if result.item and result.item.id == "front_door_key" and self.monster then
                self.monster:triggerEvent("event_final_chase_trigger", self.roomSystem:getCurrentRoomId())
                if self.dangerSystem then
                    self.dangerSystem:applyEvent("event_final_chase_trigger")
                end
            end
            self.notice = "Taken: " .. result.item.name
        elseif result.reason == "already_acquired" then
            self.notice = "Already taken"
        elseif result.itemUsed then
            local puzzle = self.puzzleSystem:getPuzzleForObject(result.object)
            if puzzle and puzzle.type == "item_use" then
                self:handlePuzzleResult(self.puzzleSystem:useItemOnObject(result.object, result.item.id))
            else
                self.notice = "Used: " .. result.item.name
            end
        elseif result.blocked and result.reason == "wrong_target" then
            self.notice = "No reaction"
        elseif result.blocked and result.object and result.object.type == "puzzle_object" then
            local puzzle = self.puzzleSystem:getPuzzleForObject(result.object)
            local canOpen, reason = self.puzzleSystem:canOpen(puzzle)

            if canOpen and not self.puzzleSystem:isSolved(puzzle.id) then
                self.puzzleUi:openPuzzle(puzzle)
                if self.inventoryUi then
                    self.inventoryUi:close()
                end
                if self.mapUi then
                    self.mapUi:close()
                end
                self.notice = nil
            elseif canOpen then
                self.notice = "Already solved"
            elseif reason == "missing_required_item" then
                self.notice = "Something is missing"
            else
                self.notice = "No reaction"
            end
        elseif result.blocked and result.reason == "locked" then
            self.notice = "Locked"
        elseif result.blocked and result.reason == "escape_locked" then
            self.notice = "Locked"
        end
    end
end

function GameScene:mousemoved(x, y, dx, dy)
    if self.hidingSystem then
        self.hidingSystem:mousemoved(x, y, dx, dy)
    end
end

return GameScene
