local Input = {}
Input.__index = Input

function Input.new()
    return setmetatable({
        clicks = {}
    }, Input)
end

function Input:mousepressed(x, y, button)
    if button ~= 1 then
        return
    end

    self.clicks[#self.clicks + 1] = {
        x = x,
        y = y,
        button = button
    }
end

function Input:consumeClicks()
    local clicks = self.clicks
    self.clicks = {}
    return clicks
end

function Input:clear()
    self.clicks = {}
end

return Input
