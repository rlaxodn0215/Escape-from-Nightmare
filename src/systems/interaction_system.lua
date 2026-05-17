local InteractionSystem = {}
InteractionSystem.__index = InteractionSystem

local MOVEMENT_TYPES = {
    door = true,
    edge_navigation = true
}

local function contains(hitbox, x, y)
    return x >= hitbox.x
        and x <= hitbox.x + hitbox.w
        and y >= hitbox.y
        and y <= hitbox.y + hitbox.h
end

function InteractionSystem.new(objectsByRoom)
    return setmetatable({
        objectsByRoom = objectsByRoom or {}
    }, InteractionSystem)
end

function InteractionSystem:getObjects(roomId)
    return self.objectsByRoom[roomId] or {}
end

function InteractionSystem:findObjectAt(roomId, x, y)
    local objects = self:getObjects(roomId)

    for index = #objects, 1, -1 do
        local object = objects[index]
        if object.hitbox and contains(object.hitbox, x, y) then
            return object
        end
    end

    return nil
end

function InteractionSystem:handleClick(roomSystem, x, y, inventorySystem)
    local object = self:findObjectAt(roomSystem:getCurrentRoomId(), x, y)

    if not object then
        return { handled = false }
    end

    if inventorySystem and inventorySystem:getSelectedItemId() then
        local useResult = inventorySystem:useSelectedOn(object)

        if useResult.used then
            return {
                handled = true,
                itemUsed = true,
                item = useResult.item,
                object = object
            }
        end

        return {
            handled = true,
            blocked = true,
            object = object,
            item = useResult.item,
            reason = useResult.reason
        }
    end

    if object.type == "item_pickup" then
        if inventorySystem then
            local acquired, reason, item = inventorySystem:addItem(object.itemId)
            return {
                handled = true,
                itemPickup = acquired,
                object = object,
                item = item,
                reason = reason
            }
        end

        return { handled = true, blocked = true, object = object, reason = "inventory_unavailable" }
    end

    if object.type == "locked_door" then
        return { handled = true, blocked = true, object = object, reason = "locked" }
    end

    if object.type == "escape_door" then
        return { handled = true, blocked = true, object = object, reason = "escape_locked" }
    end

    if MOVEMENT_TYPES[object.type] and roomSystem:moveTo(object.targetRoom) then
        return { handled = true, moved = true, object = object, roomId = roomSystem:getCurrentRoomId() }
    end

    return { handled = true, blocked = true, object = object, reason = "unavailable" }
end

return InteractionSystem
