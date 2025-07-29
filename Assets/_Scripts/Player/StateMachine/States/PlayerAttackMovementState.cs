using UnityEngine;
using Animancer;
// 3. Attack Movement State - Slight forward momentum during attacks
public class PlayerAttackMovementState : BasePlayerState
{
    private AnimancerState _currentState;
    private Vector3 _attackMomentum;
    private float _momentumTimer;
    private bool _hasMomentum;
    private AttackAnimationData _attackData;

    public PlayerAttackMovementState(PlayerBrain brain) : base(brain)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        _attackData = _brain.Animations.playerAttackMovement;
        _currentState = _brain.Animancer.Play(_attackData.animation);
        _currentState.Events(_attackData.animation).OnEnd = CompleteAttack;

        // Calculate attack momentum based on input direction
        _attackMomentum = CalculateAttackMomentum();
        _momentumTimer = 0f;
        _hasMomentum = !_attackData.useRootMotion;

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

        if (!_attackData.useRootMotion && _hasMomentum)
        {
            // Apply manual attack momentum
            _momentumTimer += deltaTime;

            // Momentum decreases over time based on attack data
            float momentumPhase = Mathf.Clamp01(_momentumTimer / (_currentState.Length * _attackData.momentumDuration));
            float momentumMultiplier = Mathf.Lerp(1f, 0.2f, momentumPhase);

            Vector3 movement = _attackMomentum * momentumMultiplier;
            _brain.CharacterController.Move(movement * deltaTime);

            // Stop momentum after specified duration
            if (momentumPhase >= 1f)
            {
                _hasMomentum = false;
            }
        }

        // Allow attack canceling based on attack data
        if (_currentState.NormalizedTime > _attackData.cancelableFromNormalizedTime)
        {
            HandleAttackCanceling();
        }
    }

    public override void ExitState()
    {
        base.ExitState();
        _brain.Animancer.Animator.applyRootMotion = true;

        // End attack effects
        if (_brain.VisualEffects != null)
        {
            _brain.VisualEffects.OnAttackEnd();
        }
    }

    private Vector3 CalculateAttackMomentum()
    {
        Vector2 inputDirection = _brain.InputHandler.MovementValue;

        // Default forward momentum using attack data
        Vector3 momentum = _brain.transform.forward * _attackData.forwardMomentum;

        // Modify based on input direction
        if (inputDirection != Vector2.zero)
        {
            Vector3 forward = _brain.MainCameraTransform.forward;
            Vector3 right = _brain.MainCameraTransform.right;
            forward.y = 0; forward.Normalize();
            right.y = 0; right.Normalize();

            Vector3 inputWorldDirection = (forward * inputDirection.y + right * inputDirection.x).normalized;
            momentum = inputWorldDirection * _attackData.forwardMomentum;
        }

        return momentum;
    }

    private void HandleAttackCanceling()
    {
        // Allow canceling with dodge
        if (_brain.InputHandler.DashPressed)
        {
            if (_brain.Combat.IsInCombat)
                _brain.FSM.SwitchState(_brain.FSM.combatDodgeState);
            else
                _brain.FSM.SwitchState(_brain.FSM.dashState);
        }
        // Allow canceling with movement (combat strafe)
        else if (_brain.InputHandler.MovementValue != Vector2.zero && _brain.Combat.IsInCombat)
        {
            _brain.FSM.SwitchState(_brain.FSM.combatStrafeState);
        }
    }

    // Public method for input handler to check if this attack can be canceled
    public bool CanCancel()
    {
        return _currentState != null && _currentState.NormalizedTime > _attackData.cancelableFromNormalizedTime;
    }

    private void CompleteAttack()
    {
        // Transition based on current context
        if (_brain.Combat.IsInCombat)
        {
            if (_brain.InputHandler.MovementValue != Vector2.zero)
                _brain.FSM.SwitchState(_brain.FSM.combatStrafeState);
            else
                _brain.FSM.SwitchState(_brain.FSM.combatIdleState);
        }
        else
        {
            if (_brain.InputHandler.MovementValue != Vector2.zero)
                _brain.FSM.SwitchState(_brain.FSM.runState);
            else
                _brain.FSM.SwitchState(_brain.FSM.idleState);
        }
    }
}
