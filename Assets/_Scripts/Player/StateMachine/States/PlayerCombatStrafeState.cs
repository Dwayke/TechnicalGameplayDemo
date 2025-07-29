using UnityEngine;
// 1. Combat Strafe State - Slower, more precise movement during combat
public class PlayerCombatStrafeState : BasePlayerState
{
    private bool _isInCombat;
    private float _combatTimer;
    private const float COMBAT_TIMEOUT = 3f;

    public PlayerCombatStrafeState(PlayerBrain brain) : base(brain)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        _brain.Animancer.Play(_brain.Animations.playerCombatStrafe);
        _isInCombat = true;
        _combatTimer = 0f;
    }

    public override void UpdateState(float deltaTime)
    {
        base.UpdateState(deltaTime);

        _combatTimer += deltaTime;

        if (!IsNearEnemies() && _combatTimer > COMBAT_TIMEOUT)
        {
            _isInCombat = false;
        }

        Vector3 movement = CalculateCombatMovement();
        float combatSpeedMultiplier = _brain.Locomotion.GetMovementSpeedMultiplier(MovementContext.Combat);
        _brain.CharacterController.Move(_brain.Locomotion.FreeLookMovementSpeed * combatSpeedMultiplier * deltaTime * movement);

        if (_brain.InputHandler.MovementValue == Vector2.zero)
        {
            if (_isInCombat)
                _brain.FSM.SwitchState(_brain.FSM.combatIdleState);
            else
                _brain.FSM.SwitchState(_brain.FSM.idleState);
        }
        else if (!_isInCombat)
        {
            _brain.FSM.SwitchState(_brain.FSM.runState);
        }
        else
        {
            // MODIFIED: Different rotation handling for combat strafe
            HandleCombatRotation();
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private Vector3 CalculateCombatMovement()
    {
        Vector3 forward = _brain.MainCameraTransform.forward;
        Vector3 right = _brain.MainCameraTransform.right;

        if (_brain.Combat.HasTarget)
        {
            Vector3 toTarget = (_brain.Combat.CurrentTarget.position - _brain.transform.position);
            toTarget.y = 0;
            toTarget.Normalize();

            forward = toTarget;
            right = Vector3.Cross(Vector3.up, forward);
        }

        forward.y = 0; forward.Normalize();
        right.y = 0; right.Normalize();

        Vector2 input = _brain.InputHandler.MovementValue;
        return (forward * input.y + right * input.x).normalized;
    }

    private void HandleCombatRotation()
    {
        if (_brain.Combat.HasTarget)
        {
            Vector2 input = _brain.InputHandler.MovementValue;
            Vector3 lookDirection = (_brain.Combat.CurrentTarget.position - _brain.transform.position);
            lookDirection.y = 0;
            lookDirection.Normalize();

            // In combat, always face the target when moving forward/backward
            // But allow facing left/right when pure strafing
            if (Mathf.Abs(input.y) > 0.1f) // Forward/backward movement toward/away from target
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                _brain.transform.rotation = Quaternion.Slerp(_brain.transform.rotation, targetRotation, 8f * Time.deltaTime);
            }
            else if (Mathf.Abs(input.x) > 0.1f && Mathf.Abs(input.y) < 0.1f) // Pure strafe
            {
                // Face perpendicular to target for strafing
                Vector3 strafeDirection = input.x > 0 ? Vector3.Cross(Vector3.up, lookDirection) : Vector3.Cross(lookDirection, Vector3.up);
                Quaternion targetRotation = Quaternion.LookRotation(strafeDirection);
                _brain.transform.rotation = Quaternion.Slerp(_brain.transform.rotation, targetRotation, 6f * Time.deltaTime);
            }
            // For mixed input, gradually blend toward target
            else if (input.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                _brain.transform.rotation = Quaternion.Slerp(_brain.transform.rotation, targetRotation, 4f * Time.deltaTime);
            }
        }
    }

    private bool IsNearEnemies()
    {
        Collider[] enemies = Physics.OverlapSphere(_brain.transform.position, 10f, _brain.Combat.EnemyLayerMask);
        return enemies.Length > 0;
    }
}
