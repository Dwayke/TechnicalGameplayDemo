using UnityEngine;

public class PlayerFSM : MonoBehaviour
{
    protected BasePlayerState _defaultState;

    // Basic states
    public PlayerIdleState idleState;
    public PlayerRunState runState;
    public PlayerJumpState jumpState;
    public PlayerDashState dashState;
    public PlayerAttackState attackState;

    // Combat movement states
    public PlayerCombatIdleState combatIdleState;
    public PlayerCombatStrafeState combatStrafeState;
    public PlayerCombatDodgeState combatDodgeState;
    public PlayerAttackMovementState attackMovementState;

    public BasePlayerState currentState;
    private PlayerBrain _brain;

    public virtual void Initialize()
    {
        _brain = GetComponent<PlayerBrain>();

        // Initialize basic states
        idleState = new PlayerIdleState(_brain);
        jumpState = new PlayerJumpState(_brain);
        dashState = new PlayerDashState(_brain);
        runState = new PlayerRunState(_brain);
        attackState = new PlayerAttackState(_brain);

        // Initialize combat states
        combatIdleState = new PlayerCombatIdleState(_brain);
        combatStrafeState = new PlayerCombatStrafeState(_brain);
        combatDodgeState = new PlayerCombatDodgeState(_brain);
        attackMovementState = new PlayerAttackMovementState(_brain);

        SetUpDefaultState();
        SwitchState(_defaultState);
    }

    public void UpdateState()
    {
        currentState?.UpdateState(Time.deltaTime);
    }

    public void SwitchState(BasePlayerState state)
    {
        currentState?.ExitState();
        currentState = state;
        currentState?.EnterState();
    }

    public void SetUpDefaultState()
    {
        _defaultState = idleState;
    }

    // Helper methods for common state transitions
    public void EnterCombat()
    {
        // Switch to appropriate combat state based on current movement
        if (currentState == runState)
            SwitchState(combatStrafeState);
        else
            SwitchState(combatIdleState);
    }

    public void ExitCombat()
    {
        // Switch to appropriate normal state
        if (_brain.InputHandler.MovementValue != Vector2.zero)
            SwitchState(runState);
        else
            SwitchState(idleState);
    }

    public bool IsInCombatState()
    {
        return currentState == combatIdleState ||
               currentState == combatStrafeState ||
               currentState == combatDodgeState ||
               currentState == attackMovementState;
    }
}