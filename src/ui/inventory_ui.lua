local InventoryUi = {}
InventoryUi.__index = InventoryUi

local PANEL = { x = 296, y = 116, w = 688, h = 460 }
local CLOSE_BUTTON = { x = 936, y = 128, w = 28, h = 28 }
local SLOT = { w = 188, h = 58 }

local function contains(rect, x, y)
    return x >= rect.x and x <= rect.x + rect.w and y >= rect.y and y <= rect.y + rect.h
end

function InventoryUi.new(inventorySystem)
    return setmetatable({
        inventorySystem = inventorySystem,
        open = false,
        slots = {}
    }, InventoryUi)
end

function InventoryUi:isOpen()
    return self.open
end

function InventoryUi:toggle()
    self.open = not self.open
end

function InventoryUi:close()
    self.open = false
end

function InventoryUi:handleClick(x, y)
    if not self.open then
        return { handled = false }
    end

    if contains(CLOSE_BUTTON, x, y) then
        self:close()
        return { handled = true, closed = true }
    end

    for _, slot in ipairs(self.slots) do
        if contains(slot, x, y) then
            self.inventorySystem:selectItem(slot.item.id)
            self:close()
            return { handled = true, selectedItem = slot.item }
        end
    end

    if contains(PANEL, x, y) then
        return { handled = true }
    end

    self:close()
    return { handled = true, closed = true }
end

function InventoryUi:draw()
    if not self.open then
        return
    end

    love.graphics.setColor(0, 0, 0, 0.58)
    love.graphics.rectangle("fill", 0, 0, 1280, 720)

    love.graphics.setColor(0.045, 0.045, 0.05, 0.97)
    love.graphics.rectangle("fill", PANEL.x, PANEL.y, PANEL.w, PANEL.h)
    love.graphics.setColor(0.35, 0.35, 0.35, 1)
    love.graphics.rectangle("line", PANEL.x, PANEL.y, PANEL.w, PANEL.h)

    love.graphics.setColor(0.72, 0.72, 0.72, 1)
    love.graphics.printf("Inventory", PANEL.x, PANEL.y + 18, PANEL.w, "center")

    love.graphics.setColor(0.18, 0.18, 0.18, 1)
    love.graphics.rectangle("line", CLOSE_BUTTON.x, CLOSE_BUTTON.y, CLOSE_BUTTON.w, CLOSE_BUTTON.h)
    love.graphics.setColor(0.72, 0.72, 0.72, 1)
    love.graphics.printf("X", CLOSE_BUTTON.x, CLOSE_BUTTON.y + 6, CLOSE_BUTTON.w, "center")

    self.slots = {}

    local selectedItemId = self.inventorySystem:getSelectedItemId()
    local items = self.inventorySystem:getItems()

    if #items == 0 then
        love.graphics.setColor(0.42, 0.42, 0.42, 1)
        love.graphics.printf("Empty", PANEL.x, PANEL.y + 214, PANEL.w, "center")
        return
    end

    for index, item in ipairs(items) do
        local column = (index - 1) % 3
        local row = math.floor((index - 1) / 3)
        local slot = {
            x = PANEL.x + 46 + column * 214,
            y = PANEL.y + 76 + row * 78,
            w = SLOT.w,
            h = SLOT.h,
            item = item
        }

        self.slots[#self.slots + 1] = slot

        love.graphics.setColor(0.075, 0.075, 0.082, 1)
        love.graphics.rectangle("fill", slot.x, slot.y, slot.w, slot.h)

        if selectedItemId == item.id then
            love.graphics.setColor(0.48, 0.08, 0.08, 1)
        else
            love.graphics.setColor(0.24, 0.24, 0.24, 1)
        end

        love.graphics.rectangle("line", slot.x, slot.y, slot.w, slot.h)
        love.graphics.setColor(0.7, 0.7, 0.7, 1)
        love.graphics.printf(item.name, slot.x + 10, slot.y + 20, slot.w - 20, "left")
    end
end

return InventoryUi
