local Fsm = {}
Fsm.__index = Fsm

Fsm.STATES = {
    NORMAL = "NORMAL",
    APPROACHING = "APPROACHING",
    SEARCHING = "SEARCHING",
    NEAR_DETECTION = "NEAR_DETECTION",
    CHASE = "CHASE"
}

local VALID_TRANSITIONS = {
    NORMAL = {
        APPROACHING = true,
        SEARCHING = true,
        CHASE = true
    },
    APPROACHING = {
        NORMAL = true,
        SEARCHING = true,
        NEAR_DETECTION = true,
        CHASE = true
    },
    SEARCHING = {
        NORMAL = true,
        APPROACHING = true,
        NEAR_DETECTION = true,
        CHASE = true
    },
    NEAR_DETECTION = {
        NORMAL = true,
        SEARCHING = true,
        CHASE = true
    },
    CHASE = {
        NORMAL = true
    }
}

function Fsm.new(initialState)
    local state = initialState or Fsm.STATES.NORMAL
    assert(Fsm.STATES[state] == state, "Unknown monster FSM state: " .. tostring(state))

    return setmetatable({
        state = state,
        previousState = nil,
        changed = false
    }, Fsm)
end

function Fsm:getState()
    return self.state
end

function Fsm:canTransitionTo(nextState)
    return Fsm.STATES[nextState] == nextState
        and (self.state == nextState or VALID_TRANSITIONS[self.state][nextState] == true)
end

function Fsm:transitionTo(nextState)
    if not self:canTransitionTo(nextState) then
        return false, "invalid_transition"
    end

    self.changed = self.state ~= nextState
    self.previousState = self.state
    self.state = nextState

    return true
end

function Fsm:resetChanged()
    self.changed = false
end

return Fsm
