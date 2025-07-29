using Animancer;
using UnityEngine;

public class PlayerAttackState : BasePlayerState
{
    private AnimancerState _currentState;
    private Vector2 _queuedMovement;
    private bool _canCancel = false;
    private AttackAnimationData _attackData;

    public PlayerAttackState(PlayerBrain brain) : base(brain)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        _attackData = _brain.Animations.playerAttack;
        _currentState = _brain.Animancer.Play(_attackData.animation);
        _currentState.Events(_attackData.animation).OnEnd = InitiateIdleState;

        // Reset cancel flag
        _canCancel = false;
        _queuedMovement = Vector2.zero;

        // Set root motion based on attack data
        _brain.Animancer.Animator.applyRootMotion = _attackData.useRootMotion;

        // Visual and audio effects
        if (_brain.VisualEffects != null)
        {
            _brain.VisualEffects.OnAttackStart(_attackData.isHeavyAttack);
        }

        if (_brain.AudioManager != null)
        {
            _brain.AudioManager.PlayAttackSound(_attackData.isHeavyAttack);
        }
    }

    public override void UpdateState(float deltaTime)
    {
        base.UpdateState(deltaTime);

        // Check if we've reached the cancelable portion of the attack
        if (_currentState.NormalizedTime > _attackData.cancelableFromNormalizedTime)
        {
            _canCancel = true;
        }

        // Handle root motion vs manual movement
        if (_attackData.useRootMotion)
        {
            // Root motion is handling the movement, but we can still:

            // 1. Store movement input for potential canceling or queuing
            _queuedMovement = _brain.InputHandler.MovementValue;

            // 2. Allow attack canceling with movement (if in cancelable window)
            if (_canCancel && _queuedMovement != Vector2.zero)
            {
                CancelAttackWithMovement();
                return;
            }

            // 3. Handle rotation during root motion (optional)
            HandleRotationDuringRootMotion();
        }
        else
        {
            // Manual movement logic (for attacks without root motion)
            HandleManualAttackMovement(deltaTime);
        }
    }

    public override void ExitState()
    {
        base.ExitState();

        // Ensure root motion is properly reset
        _brain.Animancer.Animator.applyRootMotion = true;

        // End attack effects
        if (_brain.VisualEffects != null)
        {
            _brain.VisualEffects.OnAttackEnd();
        }
    }

    private void InitiateIdleState()
    {
        // Check if we have queued movement to transition to run state instead
        if (_queuedMovement != Vector2.zero)
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

    private void CancelAttackWithMovement()
    {
        // Disable root motion before switching states
        _brain.Animancer.Animator.applyRootMotion = false;

        if (_brain.Combat.IsInCombat)
            _brain.FSM.SwitchState(_brain.FSM.combatStrafeState);
        else
            _brain.FSM.SwitchState(_brain.FSM.runState);
    }

    private void HandleRotationDuringRootMotion()
    {
        // Optional: Allow player to rotate during root motion attacks
        if (_queuedMovement != Vector2.zero)
        {
            Vector3 inputDirection = CalculateInputDirection();
            if (inputDirection != Vector3.zero)
            {
                // Smoothly rotate toward input direction
                Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
                _brain.transform.rotation = Quaternion.Slerp(
                    _brain.transform.rotation,
                    targetRotation,
                    5f * Time.deltaTime
                );
            }
        }
    }

    private void HandleManualAttackMovement(float deltaTime)
    {
        // For attacks without root motion, apply manual movement with momentum
        Vector3 attackMovement = Vector3.zero;

        // Add forward momentum during specified portion of the attack
        if (_currentState.NormalizedTime < _attackData.momentumDuration)
        {
            float momentumMultiplier = Mathf.Lerp(1f, 0.2f, _currentState.NormalizedTime / _attackData.momentumDuration);
            attackMovement = _brain.transform.forward * _attackData.forwardMomentum * momentumMultiplier;
        }

        // Apply the movement
        if (attackMovement != Vector3.zero)
        {
            _brain.CharacterController.Move(attackMovement * deltaTime);
        }

        // Store movement input for canceling/queuing
        _queuedMovement = _brain.InputHandler.MovementValue;

        // Allow canceling if in cancelable window
        if (_canCancel && _queuedMovement != Vector2.zero)
        {
            if (_brain.Combat.IsInCombat)
                _brain.FSM.SwitchState(_brain.FSM.combatStrafeState);
            else
                _brain.FSM.SwitchState(_brain.FSM.runState);
        }
    }

    private Vector3 CalculateInputDirection()
    {
        Vector3 forward = _brain.MainCameraTransform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = _brain.MainCameraTransform.right;
        right.y = 0;
        right.Normalize();

        Vector2 input = _queuedMovement;
        Vector3 direction = forward * input.y + right * input.x;

        return direction.normalized;
    }

    // Public method for input handler to check if this attack can be canceled
    public bool CanCancel()
    {
        return _canCancel;
    }
}