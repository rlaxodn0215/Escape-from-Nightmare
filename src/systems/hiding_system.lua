local HidingSystem = {}
HidingSystem.__index = HidingSystem

local BALANCE = {
    max_gauge = 100,
    idle_decay_per_second = 18,
    calm_decay_per_second = 8,
    speed_gain = 1.2,
    acceleration_gain = 1.7,
    exit_delay = 2.2,
    safe_after_passed_seconds = 2.8,
    detection_threshold = 78,
    severe_detection_threshold = 92,
    click_noise = 10
}

local NEAR_STATES = {
    APPROACHING = true,
    NEAR_DETECTION = true,
    CHASE = true
}

local function clamp(value, minValue, maxValue)
    if value < minValue then
        return minValue
    end

    if value > maxValue then
        return maxValue
    end

    return value
end

function HidingSystem.new(run, dangerSystem, monster)
    run = run or {}
    run.hiding = run.hiding or {}

    local system = setmetatable({
        run = run,
        dangerSystem = dangerSystem,
        monster = monster,
        state = run.hiding,
        movementImpulse = 0,
        lastSpeed = 0,
        lastMonsterNear = false
    }, HidingSystem)

    system.state.active = system.state.active == true
    system.state.gauge = system.state.gauge or 0
    system.state.canExit = system.state.canExit == true
    system.state.elapsed = system.state.elapsed or 0
    system.state.passedTimer = system.state.passedTimer or 0
    system.state.detected = system.state.detected == true

    return system
end

function HidingSystem:isActive()
    return self.state.active == true
end

function HidingSystem:getState()
    return self.state
end

function HidingSystem:getGauge()
    if self.dangerSystem then
        return math.max(self.state.gauge or 0, self.dangerSystem:getHideNoise())
    end

    return self.state.gauge or 0
end

function HidingSystem:enter(roomId, object)
    self.state.active = true
    self.state.roomId = roomId
    self.state.spotId = object and object.id or nil
    self.state.gauge = clamp(self.state.gauge or 0, 0, BALANCE.max_gauge)
    self.state.canExit = false
    self.state.elapsed = 0
    self.state.passedTimer = 0
    self.state.detected = false
    self.movementImpulse = 0
    self.lastSpeed = 0

    return { entered = true, object = object }
end

function HidingSystem:exit()
    if not self:isActive() then
        return { exited = false, reason = "not_hiding" }
    end

    if not self.state.canExit then
        self.state.gauge = clamp((self.state.gauge or 0) + BALANCE.click_noise, 0, BALANCE.max_gauge)
        if self.dangerSystem then
            self.dangerSystem:addHideNoise(BALANCE.click_noise)
        end
        return { exited = false, reason = "too_early" }
    end

    local result = {
        exited = true,
        roomId = self.state.roomId,
        spotId = self.state.spotId,
        finalChaseHideSuccess = self.state.finalChaseHideSuccess == true
    }

    self.state.active = false
    self.state.canExit = false
    self.state.elapsed = 0
    self.state.passedTimer = 0
    self.state.spotId = nil

    return result
end

function HidingSystem:mousemoved(_x, _y, dx, dy)
    if not self:isActive() then
        return
    end

    local speed = math.sqrt((dx or 0) * (dx or 0) + (dy or 0) * (dy or 0))
    local acceleration = math.abs(speed - self.lastSpeed)
    self.lastSpeed = speed
    self.movementImpulse = self.movementImpulse
        + speed * BALANCE.speed_gain
        + acceleration * BALANCE.acceleration_gain
end

function HidingSystem:isMonsterNear(currentRoomId)
    if not self.monster then
        return false
    end

    local monsterRoom = self.monster:getCurrentRoom()
    if monsterRoom and monsterRoom == currentRoomId then
        return true
    end

    return NEAR_STATES[self.monster:getState()] == true
end

function HidingSystem:markDetected()
    self.state.detected = true
    self.state.canExit = false

    if self.monster then
        self.monster:triggerEvent("event_player_captured", self.state.roomId)
    end

    if self.dangerSystem then
        self.dangerSystem:addCapture(self.dangerSystem:getBalance().max_capture)
    end
end

function HidingSystem:update(dt, currentRoomId)
    if not self:isActive() then
        return
    end

    self.state.elapsed = self.state.elapsed + dt

    if self.movementImpulse > 0 then
        local noise = self.movementImpulse * dt
        self.state.gauge = clamp((self.state.gauge or 0) + noise, 0, BALANCE.max_gauge)

        if self.dangerSystem then
            self.dangerSystem:addHideNoise(noise)
        end
    else
        self.state.gauge = clamp(
            (self.state.gauge or 0) - BALANCE.idle_decay_per_second * dt,
            0,
            BALANCE.max_gauge
        )
    end

    self.movementImpulse = 0
    self.state.gauge = clamp(
        (self.state.gauge or 0) - BALANCE.calm_decay_per_second * dt,
        0,
        BALANCE.max_gauge
    )

    local monsterNear = self:isMonsterNear(currentRoomId)
    local gauge = self:getGauge()

    if monsterNear then
        self.state.passedTimer = 0

        if gauge >= BALANCE.severe_detection_threshold
            or (self.monster and self.monster:getCurrentRoom() == currentRoomId and gauge >= BALANCE.detection_threshold) then
            self:markDetected()
        end
    else
        self.state.passedTimer = self.state.passedTimer + dt
        if self.lastMonsterNear and self.state.passedTimer >= BALANCE.safe_after_passed_seconds then
            self.state.canExit = true
        elseif self.state.elapsed >= BALANCE.exit_delay and gauge <= 35 then
            self.state.canExit = true
        end
    end

    self.lastMonsterNear = monsterNear

    if self.state.canExit
        and self.dangerSystem
        and self.dangerSystem:isFinalChase()
        and self.state.roomId == "living_room"
        and self.state.spotId == "living_room_curtain_hide" then
        self.state.finalChaseHideSuccess = true
        self.run.flags = self.run.flags or {}
        self.run.flags.final_chase_living_room_hide_success = true
    end
end

return HidingSystem
