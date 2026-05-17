local DangerGaugeUi = {}
DangerGaugeUi.__index = DangerGaugeUi

local FRAME = { x = 978, y = 638, w = 238, h = 18 }

local function clamp(value, minValue, maxValue)
    if value < minValue then
        return minValue
    end

    if value > maxValue then
        return maxValue
    end

    return value
end

function DangerGaugeUi.new(hidingSystem)
    return setmetatable({
        hidingSystem = hidingSystem
    }, DangerGaugeUi)
end

function DangerGaugeUi:draw()
    if not self.hidingSystem or not self.hidingSystem:isActive() then
        return
    end

    local state = self.hidingSystem:getState()
    local ratio = clamp(self.hidingSystem:getGauge() / 100, 0, 1)

    love.graphics.setColor(0.025, 0.025, 0.028, 0.92)
    love.graphics.rectangle("fill", FRAME.x - 12, FRAME.y - 28, FRAME.w + 24, 58)
    love.graphics.setColor(0.2, 0.2, 0.2, 1)
    love.graphics.rectangle("line", FRAME.x - 12, FRAME.y - 28, FRAME.w + 24, 58)

    if state.canExit then
        love.graphics.setColor(0.62, 0.62, 0.62, 1)
    else
        love.graphics.setColor(0.45, 0.45, 0.45, 1)
    end
    love.graphics.rectangle("line", FRAME.x, FRAME.y, FRAME.w, FRAME.h)
    love.graphics.setColor(0.5, 0.04, 0.04, 0.95)
    love.graphics.rectangle("fill", FRAME.x + 2, FRAME.y + 2, (FRAME.w - 4) * ratio, FRAME.h - 4)
end

return DangerGaugeUi
