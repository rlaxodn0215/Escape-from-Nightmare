local Danger = {}
Danger.__index = Danger

-- Placeholder balance values. Track final tuning against
-- design/08_REMAINING_TASKS.txt section 01.6 and 02.8-13.
local BALANCE = {
    max_danger = 100,
    max_noise = 100,
    max_capture = 100,
    noise_decay_per_second = 9,
    danger_decay_per_second = 0.6,
    stay_warning_seconds = 18,
    stay_danger_per_second = 0.9,
    stay_noise_per_second = 0.35,
    puzzle_failure_noise = 8,
    loud_event_noise = 18,
    final_chase_danger = 40,
    final_chase_capture_per_second = 18,
    chase_capture_per_second = 10,
    near_detection_capture_per_second = 7,
    capture_decay_per_second = 5,
    hide_noise_decay_per_second = 10
}

local WARNING_THRESHOLDS = {
    footsteps = 18,
    heartbeat = 38,
    light_flicker = 58,
    breath_growl = 76
}

local LOUD_EVENT_TYPES = {
    monster_pressure = true,
    monster_appearance = true,
    final_chase = true
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

local function ensureState(run)
    if type(run.danger) ~= "table" then
        run.danger = {}
    end

    run.danger.danger_level = run.danger.danger_level or 0
    run.danger.noise_level = run.danger.noise_level or 0
    run.danger.stay_timer = run.danger.stay_timer or 0
    run.danger.capture_gauge = run.danger.capture_gauge or 0
    run.danger.hide_noise = run.danger.hide_noise or 0
    run.danger.final_chase = run.danger.final_chase == true
    run.danger.balance_source = "design/08_REMAINING_TASKS.txt: final balance values pending"

    return run.danger
end

function Danger.new(run, events)
    run = run or {}

    local danger = setmetatable({
        run = run,
        events = events or {},
        state = ensureState(run),
        roomId = run.currentRoom,
        warningHooks = {
            footsteps = false,
            heartbeat = false,
            light_flicker = false,
            breath_growl = false
        }
    }, Danger)

    danger:updateWarningHooks()
    return danger
end

function Danger:getBalance()
    return BALANCE
end

function Danger:getState()
    return self.state
end

function Danger:getDangerLevel()
    return self.state.danger_level
end

function Danger:getNoiseLevel()
    return self.state.noise_level
end

function Danger:getStayTimer()
    return self.state.stay_timer
end

function Danger:getCaptureGauge()
    return self.state.capture_gauge
end

function Danger:getHideNoise()
    return self.state.hide_noise
end

function Danger:setPlayerRoom(roomId)
    if roomId ~= self.roomId then
        self.roomId = roomId
        self.state.stay_timer = 0
    end
end

function Danger:addDanger(amount)
    self.state.danger_level = clamp(
        self.state.danger_level + (amount or 0),
        0,
        BALANCE.max_danger
    )
    self:updateWarningHooks()
end

function Danger:addNoise(amount)
    self.state.noise_level = clamp(
        self.state.noise_level + (amount or 0),
        0,
        BALANCE.max_noise
    )
    self:updateWarningHooks()
end

function Danger:addCapture(amount)
    self.state.capture_gauge = clamp(
        self.state.capture_gauge + (amount or 0),
        0,
        BALANCE.max_capture
    )
end

function Danger:addHideNoise(amount)
    self.state.hide_noise = clamp(
        self.state.hide_noise + (amount or 0),
        0,
        BALANCE.max_noise
    )
end

function Danger:applyPuzzleFailure(puzzle)
    self:addDanger(puzzle and puzzle.danger_delta_on_failure or 0)
    self:addNoise(BALANCE.puzzle_failure_noise)
end

function Danger:applyEvent(eventOrId)
    local event = eventOrId
    if type(eventOrId) == "string" then
        event = self.events[eventOrId]
    end

    if not event then
        return
    end

    self:addDanger(event.danger_delta or 0)

    if LOUD_EVENT_TYPES[event.type] then
        self:addNoise(BALANCE.loud_event_noise)
    end

    if event.type == "final_chase" then
        self:startFinalChase()
    elseif event.type == "game_over" then
        self:addCapture(BALANCE.max_capture)
    end
end

function Danger:startFinalChase()
    self.state.final_chase = true
    self:addDanger(BALANCE.final_chase_danger)
end

function Danger:isFinalChase()
    return self.state.final_chase == true
end

function Danger:updateWarningHooks()
    local pressure = math.max(self.state.danger_level, self.state.noise_level)

    self.warningHooks.footsteps = pressure >= WARNING_THRESHOLDS.footsteps
    self.warningHooks.heartbeat = pressure >= WARNING_THRESHOLDS.heartbeat
    self.warningHooks.light_flicker = pressure >= WARNING_THRESHOLDS.light_flicker
    self.warningHooks.breath_growl = pressure >= WARNING_THRESHOLDS.breath_growl
end

function Danger:getWarningHooks()
    return {
        footsteps = self.warningHooks.footsteps,
        heartbeat = self.warningHooks.heartbeat,
        light_flicker = self.warningHooks.light_flicker,
        breath_growl = self.warningHooks.breath_growl
    }
end

function Danger:isCaptureFull()
    return self.state.capture_gauge >= BALANCE.max_capture
end

function Danger:update(dt, monsterState)
    self.state.stay_timer = self.state.stay_timer + dt

    self.state.noise_level = clamp(
        self.state.noise_level - BALANCE.noise_decay_per_second * dt,
        0,
        BALANCE.max_noise
    )
    self.state.danger_level = clamp(
        self.state.danger_level - BALANCE.danger_decay_per_second * dt,
        0,
        BALANCE.max_danger
    )
    self.state.hide_noise = clamp(
        self.state.hide_noise - BALANCE.hide_noise_decay_per_second * dt,
        0,
        BALANCE.max_noise
    )

    if self.state.stay_timer >= BALANCE.stay_warning_seconds then
        self:addDanger(BALANCE.stay_danger_per_second * dt)
        self:addNoise(BALANCE.stay_noise_per_second * dt)
    end

    if self.state.final_chase then
        self:addCapture(BALANCE.final_chase_capture_per_second * dt)
    elseif monsterState == "CHASE" then
        self:addCapture(BALANCE.chase_capture_per_second * dt)
    elseif monsterState == "NEAR_DETECTION" then
        self:addCapture(BALANCE.near_detection_capture_per_second * dt)
    else
        self:addCapture(-BALANCE.capture_decay_per_second * dt)
    end

    self:updateWarningHooks()
end

return Danger
