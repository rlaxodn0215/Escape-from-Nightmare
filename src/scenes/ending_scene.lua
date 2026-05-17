local EndingScene = {}
EndingScene.__index = EndingScene

local TITLE_BUTTON = { x = 520, y = 454, w = 240, h = 44, label = "Return to Title" }

local function contains(rect, x, y)
    return x >= rect.x and x <= rect.x + rect.w and y >= rect.y and y <= rect.y + rect.h
end

function EndingScene.new(app)
    return setmetatable({
        app = app
    }, EndingScene)
end

function EndingScene:enter()
end

function EndingScene:update(_dt)
end

function EndingScene:draw()
    love.graphics.clear(0.018, 0.018, 0.02, 1)

    love.graphics.setColor(0.78, 0.78, 0.78, 1)
    love.graphics.printf("Stage 1 Clear", 0, 286, 1280, "center")

    love.graphics.setColor(0.42, 0.42, 0.42, 1)
    love.graphics.printf("Outside, the nightmare keeps breathing.", 0, 336, 1280, "center")

    love.graphics.setColor(0.10, 0.10, 0.105, 1)
    love.graphics.rectangle("fill", TITLE_BUTTON.x, TITLE_BUTTON.y, TITLE_BUTTON.w, TITLE_BUTTON.h)
    love.graphics.setColor(0.45, 0.45, 0.45, 1)
    love.graphics.rectangle("line", TITLE_BUTTON.x, TITLE_BUTTON.y, TITLE_BUTTON.w, TITLE_BUTTON.h)
    love.graphics.setColor(0.82, 0.82, 0.82, 1)
    love.graphics.printf(TITLE_BUTTON.label, TITLE_BUTTON.x, TITLE_BUTTON.y + 13, TITLE_BUTTON.w, "center")
end

function EndingScene:mousepressed(x, y, button)
    if button == 1 and contains(TITLE_BUTTON, x, y) then
        self.app:showTitle()
    end
end

return EndingScene
