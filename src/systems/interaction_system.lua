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

function InteractionSystem:handleClick(roomSystem, x, y)
    local object = self:findObjectAt(roomSystem:getCurrentRoomId(), x, y)

    if not object then
        return { handled = false }
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
