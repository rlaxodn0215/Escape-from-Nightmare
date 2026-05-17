local PuzzleUi = {}
PuzzleUi.__index = PuzzleUi

local PANEL = { x = 262, y = 94, w = 756, h = 532 }
local CLOSE_BUTTON = { x = 966, y = 110, w = 30, h = 30 }
local SUBMIT_BUTTON = { x = 528, y = 552, w = 224, h = 42 }
local CLEAR_BUTTON = { x = 770, y = 552, w = 116, h = 42 }

local function contains(rect, x, y)
    return x >= rect.x and x <= rect.x + rect.w and y >= rect.y and y <= rect.y + rect.h
end

local function displayValue(value)
    return tostring(value):gsub("_", " ")
end

local function drawRectButton(rect, label, active)
    love.graphics.setColor(0.07, 0.07, 0.077, 1)
    love.graphics.rectangle("fill", rect.x, rect.y, rect.w, rect.h)

    if active then
        love.graphics.setColor(0.48, 0.07, 0.07, 1)
    else
        love.graphics.setColor(0.30, 0.30, 0.30, 1)
    end

    love.graphics.rectangle("line", rect.x, rect.y, rect.w, rect.h)
    love.graphics.setColor(0.72, 0.72, 0.72, 1)
    love.graphics.printf(label, rect.x + 8, rect.y + math.floor((rect.h - 14) / 2), rect.w - 16, "center")
end

function PuzzleUi.new(puzzleSystem)
    return setmetatable({
        puzzleSystem = puzzleSystem,
        open = false,
        puzzle = nil,
        sequence = {},
        matches = {},
        selectedSymbol = nil,
        buttons = {},
        symbolButtons = {},
        itemButtons = {},
        feedback = nil,
        shakeTime = 0
    }, PuzzleUi)
end

function PuzzleUi:isOpen()
    return self.open
end

function PuzzleUi:openPuzzle(puzzle)
    self.open = true
    self.puzzle = puzzle
    self.sequence = {}
    self.matches = {}
    self.selectedSymbol = nil
    self.feedback = nil
    self.shakeTime = 0
end

function PuzzleUi:close()
    self.open = false
    self.puzzle = nil
    self.feedback = nil
end

function PuzzleUi:update(dt)
    self.shakeTime = math.max(0, self.shakeTime - dt)
end

function PuzzleUi:submit()
    if not self.puzzle then
        return { handled = true }
    end

    local result = nil
    if self.puzzle.type == "symbol_item_matching" then
        result = self.puzzleSystem:evaluate(self.puzzle.id, { matches = self.matches })
    else
        result = self.puzzleSystem:evaluate(self.puzzle.id, { sequence = self.sequence })
    end

    if result.solved then
        self:close()
        return { handled = true, submitted = true, result = result }
    end

    self.feedback = "Wrong"
    self.shakeTime = 0.24
    return { handled = true, submitted = true, result = result }
end

function PuzzleUi:handleClick(x, y)
    if not self.open then
        return { handled = false }
    end

    if contains(CLOSE_BUTTON, x, y) then
        self:close()
        return { handled = true, closed = true }
    end

    if contains(SUBMIT_BUTTON, x, y) then
        return self:submit()
    end

    if contains(CLEAR_BUTTON, x, y) then
        self.sequence = {}
        self.matches = {}
        self.selectedSymbol = nil
        self.feedback = nil
        return { handled = true, cleared = true }
    end

    if self.puzzle and self.puzzle.type == "symbol_item_matching" then
        for _, rect in ipairs(self.symbolButtons) do
            if contains(rect, x, y) then
                self.selectedSymbol = rect.symbol
                return { handled = true }
            end
        end

        for _, rect in ipairs(self.itemButtons) do
            if contains(rect, x, y) then
                if self.selectedSymbol then
                    self.matches[self.selectedSymbol] = rect.item.id
                    self.selectedSymbol = nil
                end
                return { handled = true }
            end
        end
    else
        for _, rect in ipairs(self.buttons) do
            if contains(rect, x, y) then
                local maxLength = #(self.puzzle.answer or {})
                if #self.sequence < maxLength then
                    self.sequence[#self.sequence + 1] = rect.value
                end
                self.feedback = nil
                return { handled = true }
            end
        end
    end

    if contains(PANEL, x, y) then
        return { handled = true }
    end

    return { handled = true }
end

function PuzzleUi:drawSequencePuzzle(puzzle)
    self.buttons = {}

    local sequenceText = ""
    for index, value in ipairs(self.sequence) do
        if index > 1 then
            sequenceText = sequenceText .. "  "
        end
        sequenceText = sequenceText .. displayValue(value)
    end

    if sequenceText == "" then
        sequenceText = "...."
    end

    love.graphics.setColor(0.58, 0.58, 0.58, 1)
    love.graphics.printf(sequenceText, PANEL.x + 64, PANEL.y + 96, PANEL.w - 128, "center")

    local choices = self.puzzleSystem:getChoices(puzzle)
    local buttonW = puzzle.type == "number_lock" and 70 or 142
    local buttonH = 42
    local columns = puzzle.type == "number_lock" and 5 or 4
    local startX = PANEL.x + math.floor((PANEL.w - (columns * buttonW + (columns - 1) * 16)) / 2)
    local startY = PANEL.y + 176

    for index, value in ipairs(choices) do
        local column = (index - 1) % columns
        local row = math.floor((index - 1) / columns)
        local rect = {
            x = startX + column * (buttonW + 16),
            y = startY + row * 62,
            w = buttonW,
            h = buttonH,
            value = value
        }

        self.buttons[#self.buttons + 1] = rect
        drawRectButton(rect, displayValue(value), false)
    end
end

function PuzzleUi:drawMatchingPuzzle(puzzle)
    self.symbolButtons = {}
    self.itemButtons = {}

    local symbols = {}
    for symbol, _itemId in pairs(puzzle.answer or {}) do
        symbols[#symbols + 1] = symbol
    end
    table.sort(symbols)

    local items = {}
    for _, itemId in ipairs(puzzle.required_items or {}) do
        local item = self.puzzleSystem.inventorySystem:getItem(itemId)
        if item then
            items[#items + 1] = item
        end
    end

    love.graphics.setColor(0.50, 0.50, 0.50, 1)
    love.graphics.printf("Match each mark with an item.", PANEL.x, PANEL.y + 82, PANEL.w, "center")

    for index, symbol in ipairs(symbols) do
        local rect = { x = PANEL.x + 84, y = PANEL.y + 136 + (index - 1) * 66, w = 236, h = 46, symbol = symbol }
        self.symbolButtons[#self.symbolButtons + 1] = rect
        drawRectButton(rect, displayValue(symbol), self.selectedSymbol == symbol)

        local matched = self.matches[symbol]
        love.graphics.setColor(0.46, 0.46, 0.46, 1)
        love.graphics.printf(matched and displayValue(matched) or "...", PANEL.x + 342, rect.y + 14, 160, "left")
    end

    for index, item in ipairs(items) do
        local rect = { x = PANEL.x + 514, y = PANEL.y + 136 + (index - 1) * 66, w = 188, h = 46, item = item }
        self.itemButtons[#self.itemButtons + 1] = rect
        drawRectButton(rect, item.name, false)
    end
end

function PuzzleUi:draw()
    if not self.open or not self.puzzle then
        return
    end

    love.graphics.setColor(0, 0, 0, 0.62)
    love.graphics.rectangle("fill", 0, 0, 1280, 720)

    local offsetX = self.shakeTime > 0 and math.sin(self.shakeTime * 80) * 4 or 0

    love.graphics.push()
    love.graphics.translate(offsetX, 0)
    love.graphics.setColor(0.035, 0.035, 0.04, 0.98)
    love.graphics.rectangle("fill", PANEL.x, PANEL.y, PANEL.w, PANEL.h)
    love.graphics.setColor(0.34, 0.34, 0.34, 1)
    love.graphics.rectangle("line", PANEL.x, PANEL.y, PANEL.w, PANEL.h)

    love.graphics.setColor(0.72, 0.72, 0.72, 1)
    love.graphics.printf(displayValue(self.puzzle.id), PANEL.x, PANEL.y + 24, PANEL.w, "center")

    drawRectButton(CLOSE_BUTTON, "X", false)
    drawRectButton(SUBMIT_BUTTON, "Submit", false)
    drawRectButton(CLEAR_BUTTON, "Clear", false)

    if self.puzzle.type == "symbol_item_matching" then
        self:drawMatchingPuzzle(self.puzzle)
    else
        self:drawSequencePuzzle(self.puzzle)
    end

    if self.feedback then
        love.graphics.setColor(0.55, 0.08, 0.08, 1)
        love.graphics.printf(self.feedback, PANEL.x, PANEL.y + 500, PANEL.w, "center")
    end

    love.graphics.pop()
end

return PuzzleUi
