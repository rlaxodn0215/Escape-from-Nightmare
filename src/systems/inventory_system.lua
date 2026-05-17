local InventorySystem = {}
InventorySystem.__index = InventorySystem

local function listContains(list, value)
    if not list then
        return false
    end

    for _, entry in ipairs(list) do
        if entry == value then
            return true
        end
    end

    return false
end

function InventorySystem.new(itemData, state)
    state = state or {}
    state.items = state.items or {}
    state.order = state.order or {}
    state.flags = state.flags or {}
    state.selectedItemId = state.selectedItemId
    state.combineFirstItemId = state.combineFirstItemId

    return setmetatable({
        itemData = itemData or {},
        state = state
    }, InventorySystem)
end

function InventorySystem:getItems()
    local items = {}

    for _, itemId in ipairs(self.state.order) do
        if self.state.items[itemId] then
            items[#items + 1] = self.itemData[itemId]
        end
    end

    return items
end

function InventorySystem:getItem(itemId)
    return self.itemData[itemId]
end

function InventorySystem:hasItem(itemId)
    return self.state.items[itemId] == true
end

function InventorySystem:addItem(itemId)
    local item = self.itemData[itemId]

    if not item then
        return false, "unknown_item"
    end

    if self:hasItem(itemId) then
        return false, "already_acquired"
    end

    self.state.items[itemId] = true
    self.state.order[#self.state.order + 1] = itemId

    for _, flag in ipairs(item.flags_on_acquire or {}) do
        self.state.flags[flag] = true
    end

    return true, "acquired", item
end

function InventorySystem:selectItem(itemId)
    if not self:hasItem(itemId) then
        return false
    end

    if self.state.selectedItemId == itemId then
        self.state.selectedItemId = nil
    else
        self.state.selectedItemId = itemId
    end

    return true
end

function InventorySystem:getSelectedItemId()
    return self.state.selectedItemId
end

function InventorySystem:getSelectedItem()
    return self:getItem(self.state.selectedItemId)
end

function InventorySystem:clearSelection()
    self.state.selectedItemId = nil
end

function InventorySystem:canUseItemOn(itemId, object)
    local item = self:getItem(itemId)

    if not item or not object then
        return false
    end

    return listContains(item.used_in, object.id)
        or listContains(item.used_in, object.useTarget)
end

function InventorySystem:useSelectedOn(object)
    local itemId = self:getSelectedItemId()

    if not itemId then
        return { used = false, reason = "no_selection" }
    end

    if self:canUseItemOn(itemId, object) then
        self:clearSelection()
        return { used = true, item = self:getItem(itemId), object = object }
    end

    return { used = false, reason = "wrong_target", item = self:getItem(itemId), object = object }
end

function InventorySystem:beginCombine(itemId)
    if not self:hasItem(itemId) then
        return false
    end

    self.state.combineFirstItemId = itemId
    return true
end

function InventorySystem:combineWith(itemId)
    local firstItemId = self.state.combineFirstItemId
    self.state.combineFirstItemId = nil

    if not firstItemId or firstItemId == itemId or not self:hasItem(itemId) then
        return { combined = false }
    end

    return { combined = false, firstItemId = firstItemId, secondItemId = itemId }
end

return InventorySystem
