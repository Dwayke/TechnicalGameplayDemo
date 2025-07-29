using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour, Controls.IPlayerActions
{
    public Vector2 MovementValue { get; private set; }

    // Input state tracking for states to check
    public bool JumpPressed { get; private set; }
    public bool DashPressed { get; private set; }
    public bool AttackPressed { get; private set; }

    private Controls _controls;
    private PlayerBrain _brain;

    // Input buffer time
    private const float INPUT_BUFFER_TIME = 0.1f;
    private float _jumpBufferTimer;
    private float _dashBufferTimer;
    private float _attackBufferTimer;

    private void Start()
    {
        _controls = new Controls();
        _brain = GetComponent<PlayerBrain>();
        _controls.Player.SetCallbacks(this);
        _controls.Player.Enable();
    }

    private void Update()
    {
        // Update input buffer timers
        UpdateInputBuffers(Time.deltaTime);
    }

    private void OnDestroy()
    {
        _controls.Player.Disable();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            JumpPressed = true;
            _jumpBufferTimer = INPUT_BUFFER_TIME;

            // Direct state switching for immediate response
            if (_brain.FSM.currentState == _brain.FSM.idleState ||
                _brain.FSM.currentState == _brain.FSM.runState ||
                _brain.FSM.currentState == _brain.FSM.combatIdleState ||
                _brain.FSM.currentState == _brain.FSM.combatStrafeState)
            {
                _brain.FSM.SwitchState(_brain.FSM.jumpState);
            }
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DashPressed = true;
            _dashBufferTimer = INPUT_BUFFER_TIME;

            // Direct state switching - prioritize dodge in combat, dash otherwise
            if (_brain.Combat.IsInCombat)
            {
                // Switch to combat dodge if in combat
                if (CanDodge())
                {
                    _brain.FSM.SwitchState(_brain.FSM.combatDodgeState);
                }
            }
            else
            {
                // Normal dash behavior
                if (CanDash())
                {
                    _brain.FSM.SwitchState(_brain.FSM.dashState);
                }
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementValue = context.ReadValue<Vector2>();

        // Only switch to run/strafe state if we're in appropriate idle states and there's movement input
        if (MovementValue != Vector2.zero)
        {
            if (_brain.FSM.currentState == _brain.FSM.idleState)
            {
                _brain.FSM.SwitchState(_brain.FSM.runState);
            }
            else if (_brain.FSM.currentState == _brain.FSM.combatIdleState)
            {
                _brain.FSM.SwitchState(_brain.FSM.combatStrafeState);
            }
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            AttackPressed = true;
            _attackBufferTimer = INPUT_BUFFER_TIME;

            // Direct state switching for immediate response
            if (CanAttack())
            {
                if (_brain.Combat.IsInCombat)
                    _brain.FSM.SwitchState(_brain.FSM.attackMovementState);
                else
                    _brain.FSM.SwitchState(_brain.FSM.attackState);
            }
        }
    }

    // Helper methods for states to check input availability
    public bool ConsumeJumpInput()
    {
        if (JumpPressed)
        {
            JumpPressed = false;
            _jumpBufferTimer = 0f;
            return true;
        }
        return false;
    }

    public bool ConsumeDashInput()
    {
        if (DashPressed)
        {
            DashPressed = false;
            _dashBufferTimer = 0f;
            return true;
        }
        return false;
    }

    public bool ConsumeAttackInput()
    {
        if (AttackPressed)
        {
            AttackPressed = false;
            _attackBufferTimer = 0f;
            return true;
        }
        return false;
    }

    private void UpdateInputBuffers(float deltaTime)
    {
        // Decay input buffers
        if (_jumpBufferTimer > 0f)
        {
            _jumpBufferTimer -= deltaTime;
            if (_jumpBufferTimer <= 0f)
            {
                JumpPressed = false;
            }
        }

        if (_dashBufferTimer > 0f)
        {
            _dashBufferTimer -= deltaTime;
            if (_dashBufferTimer <= 0f)
            {
                DashPressed = false;
            }
        }

        if (_attackBufferTimer > 0f)
        {
            _attackBufferTimer -= deltaTime;
            if (_attackBufferTimer <= 0f)
            {
                AttackPressed = false;
            }
        }
    }

    private bool CanDash()
    {
        // Define which states allow dashing
        return _brain.FSM.currentState == _brain.FSM.idleState ||
               _brain.FSM.currentState == _brain.FSM.runState;
    }

    private bool CanDodge()
    {
        // Define which states allow dodging (more permissive for combat)
        return _brain.FSM.currentState == _brain.FSM.combatIdleState ||
               _brain.FSM.currentState == _brain.FSM.combatStrafeState ||
               (_brain.FSM.currentState == _brain.FSM.attackMovementState &&
                ((PlayerAttackMovementState)_brain.FSM.currentState).CanCancel());
    }

    private bool CanAttack()
    {
        // Define which states allow attacking
        return _brain.FSM.currentState == _brain.FSM.idleState ||
               _brain.FSM.currentState == _brain.FSM.runState ||
               _brain.FSM.currentState == _brain.FSM.combatIdleState ||
               _brain.FSM.currentState == _brain.FSM.combatStrafeState;
    }
}