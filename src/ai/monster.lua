local Fsm = require("src.ai.fsm")

local Monster = {}
Monster.__index = Monster

local function findNodeForRoom(nodes, roomId)
    for nodeId, node in pairs(nodes or {}) do
        if node.room == roomId then
            return nodeId
        end
    end

    return nil
end

local function listContains(list, value)
    for _, item in ipairs(list or {}) do
        if item == value then
            return true
        end
    end

    return false
end

local function copyList(list)
    local copied = {}

    for _, value in ipairs(list or {}) do
        copied[#copied + 1] = value
    end

    return copied
end

function Monster.new(monsterNodes, events, run)
    run = run or {}
    run.monster = run.monster or {}
    run.events = run.events or {}
    run.flags = run.flags or {}

    local nodes = monsterNodes.nodes or {}
    local startNode = run.monster.currentNode or monsterNodes.start_node
    assert(nodes[startNode], "Unknown monster start node: " .. tostring(startNode))

    local monster = setmetatable({
        nodes = nodes,
        eventSpawns = monsterNodes.event_spawns or {},
        finalChasePath = copyList(monsterNodes.final_chase_path),
        events = events or {},
        run = run,
        fsm = Fsm.new(run.monster.state or Fsm.STATES.NORMAL),
        active = run.monster.active == true,
        currentNode = startNode,
        targetNode = run.monster.targetNode,
        playerRoom = run.currentRoom,
        lastSoundRoom = run.monster.lastSoundRoom,
        lastSeenRoom = run.monster.lastSeenRoom,
        captured = run.monster.captured == true
    }, Monster)

    monster:syncRunState()
    return monster
end

function Monster:syncRunState()
    self.run.monster.active = self.active
    self.run.monster.state = self.fsm:getState()
    self.run.monster.currentNode = self.currentNode
    self.run.monster.targetNode = self.targetNode
    self.run.monster.playerRoom = self.playerRoom
    self.run.monster.lastSoundRoom = self.lastSoundRoom
    self.run.monster.lastSeenRoom = self.lastSeenRoom
    self.run.monster.captured = self.captured
end

function Monster:getState()
    return self.fsm:getState()
end

function Monster:getCurrentNode()
    return self.currentNode
end

function Monster:getCurrentRoom()
    local node = self.nodes[self.currentNode]
    return node and node.room or nil
end

function Monster:setPlayerRoom(roomId)
    self.playerRoom = roomId
    self.run.currentRoom = roomId
    self:syncRunState()
end

function Monster:setTargetRoom(roomId)
    self.targetNode = findNodeForRoom(self.nodes, roomId)
    self:syncRunState()
    return self.targetNode ~= nil
end

function Monster:markEvent(eventId)
    local event = self.events[eventId]

    self.run.events[eventId] = true

    if event then
        for _, flag in ipairs(event.flags_set or {}) do
            self.run.flags[flag] = true
        end
    end
end

function Monster:activateAtRoom(roomId)
    local nodeId = findNodeForRoom(self.nodes, roomId)

    if nodeId then
        self.currentNode = nodeId
    end

    self.active = true
    self.targetNode = nil
    self.fsm:transitionTo(Fsm.STATES.SEARCHING)
    self:syncRunState()
end

function Monster:hearSound(roomId)
    self.lastSoundRoom = roomId
    self.active = true
    self:setTargetRoom(roomId)
    self.fsm:transitionTo(Fsm.STATES.APPROACHING)
    self:syncRunState()
end

function Monster:seePlayer(roomId)
    self.lastSeenRoom = roomId
    self.active = true
    self:setTargetRoom(roomId)
    self.fsm:transitionTo(Fsm.STATES.CHASE)
    self:syncRunState()
end

function Monster:startFinalChase(roomId)
    self.active = true
    self.lastSeenRoom = roomId or self.playerRoom
    self.currentNode = findNodeForRoom(self.nodes, roomId or self.playerRoom) or self.currentNode
    self.targetNode = findNodeForRoom(self.nodes, self.playerRoom)
    self.fsm:transitionTo(Fsm.STATES.CHASE)
    self:syncRunState()
end

function Monster:capturePlayer()
    self.captured = true
    self.active = true
    self.fsm:transitionTo(Fsm.STATES.CHASE)
    self:syncRunState()
end

function Monster:triggerEvent(eventId, roomId)
    self:markEvent(eventId)

    if eventId == "event_kitchen_first_appearance" then
        self:activateAtRoom(roomId or self.eventSpawns[eventId])
    elseif eventId == "event_final_chase_trigger" then
        self:startFinalChase(roomId or self.eventSpawns[eventId])
    elseif eventId == "event_player_captured" then
        self:capturePlayer()
    elseif self.events[eventId] and self.events[eventId].type == "monster_pressure" then
        self:hearSound(roomId or self.eventSpawns[eventId] or self.playerRoom)
    end

    self:syncRunState()
end

function Monster:stepTowardTarget()
    if not self.targetNode or self.currentNode == self.targetNode then
        return false
    end

    local current = self.nodes[self.currentNode]
    if not current then
        return false
    end

    if listContains(current.neighbors, self.targetNode) then
        self.currentNode = self.targetNode
        return true
    end

    for _, neighborId in ipairs(current.neighbors or {}) do
        local neighbor = self.nodes[neighborId]
        if neighbor and listContains(neighbor.neighbors, self.targetNode) then
            self.currentNode = neighborId
            return true
        end
    end

    if current.neighbors and current.neighbors[1] and self.nodes[current.neighbors[1]] then
        self.currentNode = current.neighbors[1]
        return true
    end

    return false
end

function Monster:update(_dt)
    if not self.active then
        self:syncRunState()
        return
    end

    if self:getState() == Fsm.STATES.CHASE then
        self.targetNode = findNodeForRoom(self.nodes, self.playerRoom) or self.targetNode
        self:stepTowardTarget()
    elseif self:getState() == Fsm.STATES.APPROACHING then
        if not self:stepTowardTarget() or self.currentNode == self.targetNode then
            self.fsm:transitionTo(Fsm.STATES.SEARCHING)
        end
    elseif self:getState() == Fsm.STATES.SEARCHING and self:getCurrentRoom() == self.playerRoom then
        self.lastSeenRoom = self.playerRoom
        self.fsm:transitionTo(Fsm.STATES.NEAR_DETECTION)
    end

    self:syncRunState()
end

return Monster
