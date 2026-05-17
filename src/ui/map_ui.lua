local MapUi = {}
MapUi.__index = MapUi

local PANEL = { x = 258, y = 96, w = 764, h = 520 }
local CLOSE_BUTTON = { x = 974, y = 110, w = 28, h = 28 }
local FLOOR_BOXES = {
    ["1f"] = { x = 300, y = 158, w = 420, h = 188 },
    ["2f"] = { x = 742, y = 158, w = 238, h = 188 },
    basement = { x = 300, y = 382, w = 310, h = 176 },
    attic = { x = 642, y = 382, w = 338, h = 176 }
}
local MAP_SPACE = { w = 420, h = 240 }

local function contains(rect, x, y)
    return x >= rect.x and x <= rect.x + rect.w and y >= rect.y and y <= rect.y + rect.h
end

local function scaledRect(floorBox, roomRect)
    local scaleX = floorBox.w / MAP_SPACE.w
    local scaleY = floorBox.h / MAP_SPACE.h

    return {
        x = floorBox.x + roomRect.x * scaleX,
        y = floorBox.y + roomRect.y * scaleY,
        w = roomRect.w * scaleX,
        h = roomRect.h * scaleY
    }
end

local function scaledPoint(floorBox, point)
    return {
        x = floorBox.x + point.x * floorBox.w / MAP_SPACE.w,
        y = floorBox.y + point.y * floorBox.h / MAP_SPACE.h
    }
end

function MapUi.new(mapSystem)
    return setmetatable({
        mapSystem = mapSystem,
        open = false
    }, MapUi)
end

function MapUi:isOpen()
    return self.open
end

function MapUi:toggle()
    self.open = not self.open
end

function MapUi:close()
    self.open = false
end

function MapUi:handleClick(x, y)
    if not self.open then
        return { handled = false }
    end

    if contains(CLOSE_BUTTON, x, y) then
        self:close()
        return { handled = true, closed = true }
    end

    if contains(PANEL, x, y) then
        return { handled = true }
    end

    self:close()
    return { handled = true, closed = true }
end

function MapUi:draw()
    if not self.open then
        return
    end

    love.graphics.setColor(0, 0, 0, 0.36)
    love.graphics.rectangle("fill", 0, 0, 1280, 720)

    love.graphics.setColor(0.042, 0.042, 0.046, 0.97)
    love.graphics.rectangle("fill", PANEL.x, PANEL.y, PANEL.w, PANEL.h)
    love.graphics.setColor(0.34, 0.34, 0.34, 1)
    love.graphics.rectangle("line", PANEL.x, PANEL.y, PANEL.w, PANEL.h)

    love.graphics.setColor(0.72, 0.72, 0.72, 1)
    love.graphics.printf("Map", PANEL.x, PANEL.y + 18, PANEL.w, "center")

    love.graphics.setColor(0.18, 0.18, 0.18, 1)
    love.graphics.rectangle("line", CLOSE_BUTTON.x, CLOSE_BUTTON.y, CLOSE_BUTTON.w, CLOSE_BUTTON.h)
    love.graphics.setColor(0.72, 0.72, 0.72, 1)
    love.graphics.printf("X", CLOSE_BUTTON.x, CLOSE_BUTTON.y + 6, CLOSE_BUTTON.w, "center")

    local currentMarker = self.mapSystem:getCurrentRoomMarker()

    for _, floorId in ipairs(self.mapSystem:getFloorOrder()) do
        local floorBox = FLOOR_BOXES[floorId]
        local layout = self.mapSystem:getFloorLayout(floorId)

        love.graphics.setColor(0.058, 0.058, 0.064, 1)
        love.graphics.rectangle("fill", floorBox.x, floorBox.y, floorBox.w, floorBox.h)
        love.graphics.setColor(0.24, 0.24, 0.24, 1)
        love.graphics.rectangle("line", floorBox.x, floorBox.y, floorBox.w, floorBox.h)
        love.graphics.setColor(0.58, 0.58, 0.58, 1)
        love.graphics.print(self.mapSystem:getFloorLabel(floorId), floorBox.x + 12, floorBox.y + 10)

        for _, roomRect in pairs(layout) do
            local rect = scaledRect(floorBox, roomRect)
            love.graphics.setColor(0.092, 0.092, 0.098, 1)
            love.graphics.rectangle("fill", rect.x, rect.y, rect.w, rect.h)
            love.graphics.setColor(0.32, 0.32, 0.32, 1)
            love.graphics.rectangle("line", rect.x, rect.y, rect.w, rect.h)
        end

        if currentMarker and currentMarker.floorId == floorId then
            local marker = scaledPoint(floorBox, currentMarker)

            love.graphics.setColor(0.62, 0.08, 0.08, 1)
            love.graphics.circle("fill", marker.x, marker.y, 5)
            love.graphics.setColor(0.82, 0.18, 0.18, 1)
            love.graphics.circle("line", marker.x, marker.y, 8)
        end
    end
end

return MapUi
