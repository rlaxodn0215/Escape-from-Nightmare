local MapSystem = {}
MapSystem.__index = MapSystem

local FLOOR_ORDER = { "1f", "2f", "basement", "attic" }

local FLOOR_LABELS = {
    ["1f"] = "1F",
    ["2f"] = "2F",
    basement = "Basement",
    attic = "Attic"
}

local FLOOR_LAYOUTS = {
    ["1f"] = {
        entrance = { x = 20, y = 132, w = 42, h = 28 },
        first_floor_hallway = { x = 76, y = 108, w = 120, h = 76 },
        living_room = { x = 76, y = 56, w = 54, h = 34 },
        family_photo_room = { x = 142, y = 48, w = 52, h = 34 },
        dining_room = { x = 210, y = 108, w = 54, h = 34 },
        kitchen = { x = 278, y = 108, w = 58, h = 34 },
        laundry_room = { x = 350, y = 108, w = 50, h = 34 },
        garage = { x = 278, y = 156, w = 58, h = 34 },
        first_floor_bathroom = { x = 210, y = 156, w = 54, h = 34 },
        first_floor_storage = { x = 142, y = 198, w = 54, h = 34 },
        stairwell_1f = { x = 20, y = 84, w = 42, h = 34 }
    },
    ["2f"] = {
        stairwell_2f = { x = 20, y = 84, w = 42, h = 34 },
        second_floor_hallway = { x = 76, y = 108, w = 128, h = 76 },
        child_room = { x = 20, y = 150, w = 42, h = 34 },
        master_bedroom = { x = 76, y = 54, w = 56, h = 34 },
        dressing_room = { x = 146, y = 54, w = 52, h = 34 },
        guest_room = { x = 218, y = 108, w = 52, h = 34 },
        second_floor_bathroom = { x = 218, y = 156, w = 52, h = 34 },
        study = { x = 286, y = 108, w = 52, h = 34 },
        mirror_room = { x = 354, y = 108, w = 52, h = 34 }
    },
    basement = {
        basement_entry = { x = 56, y = 116, w = 58, h = 36 },
        basement_main = { x = 146, y = 104, w = 92, h = 58 },
        basement_storage = { x = 270, y = 72, w = 58, h = 36 },
        altar_room = { x = 270, y = 148, w = 58, h = 36 }
    },
    attic = {
        attic_main = { x = 144, y = 104, w = 94, h = 58 },
        attic_toy_storage = { x = 48, y = 104, w = 62, h = 36 },
        attic_album_storage = { x = 272, y = 104, w = 62, h = 36 }
    }
}

function MapSystem.new(roomSystem)
    return setmetatable({
        roomSystem = roomSystem,
        floors = FLOOR_LAYOUTS,
        floorOrder = FLOOR_ORDER,
        floorLabels = FLOOR_LABELS
    }, MapSystem)
end

function MapSystem:getFloorOrder()
    return self.floorOrder
end

function MapSystem:getFloorLabel(floorId)
    return self.floorLabels[floorId] or floorId
end

function MapSystem:getFloorLayout(floorId)
    return self.floors[floorId] or {}
end

function MapSystem:getCurrentRoomId()
    return self.roomSystem and self.roomSystem:getCurrentRoomId() or nil
end

function MapSystem:getCurrentFloorId()
    local room = self.roomSystem and self.roomSystem:getCurrentRoom() or nil
    return room and room.floor or nil
end

function MapSystem:getCurrentRoomMarker()
    local roomId = self:getCurrentRoomId()
    local floorId = self:getCurrentFloorId()
    local roomRect = floorId and self.floors[floorId] and self.floors[floorId][roomId] or nil

    if not roomRect then
        return nil
    end

    return {
        floorId = floorId,
        x = roomRect.x + roomRect.w * 0.5,
        y = roomRect.y + roomRect.h * 0.5
    }
end

return MapSystem
