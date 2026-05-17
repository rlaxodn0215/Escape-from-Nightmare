local PuzzleSystem = {}
PuzzleSystem.__index = PuzzleSystem

local function listContains(list, value)
    if not list then
        return false
    end

    for _, entry in ipairs(list) do
        if entry == value then
            return true
        end
    end

    return false
end

local function sameSequence(left, right)
    if not left or not right or #left ~= #right then
        return false
    end

    for index, value in ipairs(left) do
        if value ~= right[index] then
            return false
        end
    end

    return true
end

local function copyList(list)
    local copied = {}

    for _, value in ipairs(list or {}) do
        copied[#copied + 1] = value
    end

    return copied
end

function PuzzleSystem.new(puzzleInputs, events, inventorySystem, run)
    run = run or {}
    run.puzzles = run.puzzles or {}
    run.flags = run.flags or {}
    run.events = run.events or {}
    run.danger = run.danger or 0

    return setmetatable({
        puzzleInputs = puzzleInputs or {},
        events = events or {},
        inventorySystem = inventorySystem,
        run = run
    }, PuzzleSystem)
end

function PuzzleSystem:getPuzzle(puzzleId)
    return self.puzzleInputs[puzzleId]
end

function PuzzleSystem:getPuzzleForObject(object)
    if not object then
        return nil
    end

    return self:getPuzzle(object.puzzleId or object.useTarget or object.id)
end

function PuzzleSystem:isSolved(puzzleId)
    return self.run.puzzles[puzzleId] == true
end

function PuzzleSystem:canOpen(puzzle)
    if not puzzle then
        return false, "unknown_puzzle"
    end

    if puzzle.required_items then
        for _, itemId in ipairs(puzzle.required_items) do
            if not self.inventorySystem:hasItem(itemId) then
                return false, "missing_required_item", itemId
            end
        end
    end

    return true
end

function PuzzleSystem:applyEvent(eventId)
    local event = self.events[eventId]

    if not event then
        return
    end

    self.run.events[eventId] = true

    for _, flag in ipairs(event.flags_set or {}) do
        self.run.flags[flag] = true
    end

    if event.clear_record then
        self.run.clearRecords = self.run.clearRecords or {}
        self.run.clearRecords[event.clear_record] = true
    end

    if event.danger_delta then
        self.run.danger = math.max(0, self.run.danger + event.danger_delta)
    end
end

function PuzzleSystem:applyEvents(eventIds)
    for _, eventId in ipairs(eventIds or {}) do
        self:applyEvent(eventId)
    end
end

function PuzzleSystem:applyRewards(puzzle)
    local addedItems = {}
    local spawnedObjects = {}
    local rewards = puzzle.rewards or {}

    for _, itemId in ipairs(rewards.items or {}) do
        local added, _reason, item = self.inventorySystem:addItem(itemId)
        if added and item then
            addedItems[#addedItems + 1] = item
        end
    end

    for _, objectId in ipairs(rewards.objects or {}) do
        self.run.flags[objectId .. "_visible"] = true
        spawnedObjects[#spawnedObjects + 1] = objectId
    end

    return addedItems, spawnedObjects
end

function PuzzleSystem:markSolved(puzzle)
    self.run.puzzles[puzzle.id] = true
end

function PuzzleSystem:fail(puzzle)
    self:applyEvents(puzzle.failure_events)
    if puzzle.danger_delta_on_failure then
        self.run.danger = math.max(0, self.run.danger + puzzle.danger_delta_on_failure)
    end

    return {
        solved = false,
        failed = true,
        puzzle = puzzle,
        danger = self.run.danger
    }
end

function PuzzleSystem:succeed(puzzle)
    self:markSolved(puzzle)
    self:applyEvents(puzzle.success_events)
    local addedItems, spawnedObjects = self:applyRewards(puzzle)

    return {
        solved = true,
        puzzle = puzzle,
        addedItems = addedItems,
        spawnedObjects = spawnedObjects,
        clearFlag = puzzle.clear_flag
    }
end

function PuzzleSystem:evaluate(puzzleId, input)
    local puzzle = self:getPuzzle(puzzleId)

    if not puzzle then
        return { solved = false, failed = true, reason = "unknown_puzzle" }
    end

    if self:isSolved(puzzle.id) then
        return { solved = true, alreadySolved = true, puzzle = puzzle }
    end

    if puzzle.type == "number_lock"
        or puzzle.type == "symbol_sequence"
        or puzzle.type == "silent_sequence"
        or puzzle.type == "color_sequence" then
        if sameSequence(input.sequence, puzzle.answer) then
            return self:succeed(puzzle)
        end

        return self:fail(puzzle)
    end

    if puzzle.type == "symbol_item_matching" then
        local matches = input.matches or {}

        for symbol, itemId in pairs(puzzle.answer or {}) do
            if matches[symbol] ~= itemId then
                return self:fail(puzzle)
            end
        end

        return self:succeed(puzzle)
    end

    if puzzle.type == "item_use" then
        if listContains(puzzle.required_items, input.itemId) then
            return self:succeed(puzzle)
        end

        return self:fail(puzzle)
    end

    return { solved = false, failed = true, reason = "unsupported_puzzle_type", puzzle = puzzle }
end

function PuzzleSystem:useItemOnObject(object, itemId)
    local puzzle = self:getPuzzleForObject(object)

    if not puzzle or puzzle.type ~= "item_use" then
        return { solved = false, failed = true, reason = "not_item_use" }
    end

    return self:evaluate(puzzle.id, { itemId = itemId })
end

function PuzzleSystem:getChoices(puzzle)
    if puzzle.type == "number_lock" then
        return { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 }
    end

    if puzzle.type == "symbol_sequence" then
        return copyList(puzzle.symbols)
    end

    if puzzle.type == "silent_sequence" or puzzle.type == "color_sequence" then
        return copyList(puzzle.choices)
    end

    return {}
end

return PuzzleSystem
