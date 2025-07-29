using UnityEngine;
using Animancer;
// 2. Combat Dodge State - Quick directional dodge with i-frames
public class PlayerCombatDodgeState : BasePlayerState
{
    private Vector3 _dodgeDirection;
    private float _dodgeTimer;
    private bool _hasIFrames;
    private AnimancerState _currentState;

    private const float DODGE_DURATION = 0.5f;
    private const float IFRAME_DURATION = 0.3f;
    private const float DODGE_DISTANCE = 5f;

    public PlayerCombatDodgeState(PlayerBrain brain) : base(brain)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        _dodgeDirection = CalculateDodgeDirection();
        _dodgeTimer = 0f;
        _hasIFrames = true;

        // Enable invincibility frames
        _brain.Combat.SetInvincible(true);

        // Play dodge animation
        _currentState = _brain.Animancer.Play(_brain.Animations.playerDodge);

        // Disable root motion for manual dodge control
        _brain.Animancer.Animator.applyRootMotion = false;

        // Visual and audio effects
        if (_brain.VisualEffects != null)
        {
            _brain.VisualEffects.OnDodgeStart();
        }

        if (_brain.AudioManager != null)
        {
            _brain.AudioManager.PlayDodgeSound();
        }
        _hasIFrames = true;
        
        // Enable invincibility frames
        _brain.Combat.SetInvincible(true);
        
        // Play dodge animation
        _currentState = _brain.Animancer.Play(_brain.Animations.playerDodge);
        
        // Disable root motion for manual dodge control
        _brain.Animancer.Animator.applyRootMotion = false;
    }

public override void UpdateState(float deltaTime)
{
    base.UpdateState(deltaTime);

    _dodgeTimer += deltaTime;

    // Handle invincibility frames
    if (_hasIFrames && _dodgeTimer > IFRAME_DURATION)
    {
        _hasIFrames = false;
        _brain.Combat.SetInvincible(false);
    }

    // Apply dodge movement with easing
    float normalizedTime = _dodgeTimer / DODGE_DURATION;
    float easedSpeed = Mathf.Lerp(1f, 0f, normalizedTime * normalizedTime); // Quadratic ease-out

    Vector3 dodgeMovement = _dodgeDirection * (DODGE_DISTANCE * easedSpeed);
    _brain.CharacterController.Move(dodgeMovement * deltaTime);

    // Check if dodge is complete
    if (_dodgeTimer >= DODGE_DURATION)
    {
        CompleteDodge();
    }
}

public override void ExitState()
{
    base.ExitState();

    // Ensure invincibility is disabled
    _brain.Combat.SetInvincible(false);
    _brain.Animancer.Animator.applyRootMotion = true;
}

private Vector3 CalculateDodgeDirection()
{
    Vector2 inputDirection = _brain.InputHandler.MovementValue;

    // If no input, dodge backward
    if (inputDirection == Vector2.zero)
    {
        return -_brain.transform.forward;
    }

    // Calculate world direction from input
    Vector3 forward = _brain.MainCameraTransform.forward;
    Vector3 right = _brain.MainCameraTransform.right;
    forward.y = 0; forward.Normalize();
    right.y = 0; right.Normalize();

    Vector3 worldDirection = (forward * inputDirection.y + right * inputDirection.x).normalized;
    return worldDirection;
}

private void CompleteDodge()
{
    // Transition based on input and combat state
    if (_brain.InputHandler.MovementValue != Vector2.zero)
    {
        if (_brain.Combat.IsInCombat)
            _brain.FSM.SwitchState(_brain.FSM.combatStrafeState);
        else
            _brain.FSM.SwitchState(_brain.FSM.runState);
    }
    else
    {
        if (_brain.Combat.IsInCombat)
            _brain.FSM.SwitchState(_brain.FSM.combatIdleState);
        else
            _brain.FSM.SwitchState(_brain.FSM.idleState);
    }
}
}


