local RoomSystem = {}
RoomSystem.__index = RoomSystem

function RoomSystem.new(roomData, startRoomId)
    local rooms = roomData.list or {}
    local startRoom = startRoomId or roomData.startRoom

    assert(rooms[startRoom], "Unknown start room: " .. tostring(startRoom))

    return setmetatable({
        rooms = rooms,
        currentRoomId = startRoom
    }, RoomSystem)
end

function RoomSystem:getCurrentRoomId()
    return self.currentRoomId
end

function RoomSystem:getCurrentRoom()
    return self.rooms[self.currentRoomId]
end

function RoomSystem:getRoom(roomId)
    return self.rooms[roomId]
end

function RoomSystem:canMoveTo(targetRoomId)
    local currentRoom = self:getCurrentRoom()

    return targetRoomId ~= nil
        and self.rooms[targetRoomId] ~= nil
        and currentRoom ~= nil
        and currentRoom.connections ~= nil
        and currentRoom.connections[targetRoomId] ~= nil
end

function RoomSystem:moveTo(targetRoomId)
    if not self:canMoveTo(targetRoomId) then
        return false
    end

    self.currentRoomId = targetRoomId
    return true
end

return RoomSystem
