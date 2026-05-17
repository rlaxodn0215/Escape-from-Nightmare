local StateManager = {}
StateManager.__index = StateManager

function StateManager.new()
    return setmetatable({
        current = nil
    }, StateManager)
end

function StateManager:switch(nextState, ...)
    if self.current and self.current.exit then
        self.current:exit()
    end

    self.current = nextState

    if self.current and self.current.enter then
        self.current:enter(...)
    end
end

function StateManager:update(dt)
    if self.current and self.current.update then
        self.current:update(dt)
    end
end

function StateManager:draw()
    if self.current and self.current.draw then
        self.current:draw()
    end
end

function StateManager:mousepressed(x, y, button)
    if self.current and self.current.mousepressed then
        self.current:mousepressed(x, y, button)
    end
end

function StateManager:mousereleased(x, y, button)
    if self.current and self.current.mousereleased then
        self.current:mousereleased(x, y, button)
    end
end

function StateManager:mousemoved(x, y, dx, dy)
    if self.current and self.current.mousemoved then
        self.current:mousemoved(x, y, dx, dy)
    end
end

return StateManager
