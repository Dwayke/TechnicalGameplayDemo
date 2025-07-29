using UnityEngine;

public class PlayerRunState : BasePlayerState
{
    public PlayerRunState(PlayerBrain brain) : base(brain)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        _brain.Animancer.Play(_brain.Animations.playerRun);
    }

    public override void UpdateState(float deltaTime)
    {
        base.UpdateState(deltaTime);
        Vector3 movement = CalculateMovement();

        // Apply attack magnetism if available
        if (_brain.AttackMagnetism != null)
        {
            movement = _brain.AttackMagnetism.ApplyAttackMagnetism(movement);
        }

        _brain.CharacterController.Move(_brain.Locomotion.FreeLookMovementSpeed * deltaTime * movement);

        if (_brain.InputHandler.MovementValue == Vector2.zero)
        {
            _brain.FSM.SwitchState(_brain.FSM.idleState);
        }
        else
        {
            // NEW: Only rotate character when moving forward/backward
            HandleCharacterRotation(movement);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private Vector3 CalculateMovement()
    {
        Vector3 forward = _brain.MainCameraTransform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = _brain.MainCameraTransform.right;
        right.y = 0;
        right.Normalize();

        Vector2 input = _brain.InputHandler.MovementValue;
        Vector3 movement = forward * input.y + right * input.x;

        return movement.normalized;
    }

    private void HandleCharacterRotation(Vector3 movement)
    {
        Vector2 input = _brain.InputHandler.MovementValue;

        // Only rotate the character when there's forward/backward input
        // Allow strafing left/right without rotating
        if (Mathf.Abs(input.y) > 0.1f) // Forward/backward movement
        {
            _brain.transform.rotation = Quaternion.LookRotation(movement);
        }
        else if (Mathf.Abs(input.x) > 0.1f && Mathf.Abs(input.y) < 0.1f) // Pure left/right strafe
        {
            // Character faces left or right relative to camera
            Vector3 strafeDirection = input.x > 0 ? _brain.MainCameraTransform.right : -_brain.MainCameraTransform.right;
            strafeDirection.y = 0;
            strafeDirection.Normalize();
            _brain.transform.rotation = Quaternion.LookRotation(strafeDirection);
        }
        // For diagonal movement, use the full movement direction
        else if (input.magnitude > 0.1f)
        {
            _brain.transform.rotation = Quaternion.LookRotation(movement);
        }
    }
}