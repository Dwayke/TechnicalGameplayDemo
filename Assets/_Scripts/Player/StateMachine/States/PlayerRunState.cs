using UnityEngine;
using UnityEngine.Lumin;

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
        _brain.CharacterController.Move(_brain.Locomotion.FreeLookMovementSpeed * deltaTime * movement);
        if (_brain.InputHandler.MovementValue == Vector2.zero)
        {
            _brain.FSM.SwitchState(_brain.FSM.idleState);
        }
        else
        {
            _brain.transform.rotation = Quaternion.LookRotation(movement);
        }
    }
    public override void ExitState()
    {
        base.ExitState();
    }

    private Vector3 CalculateMovement()
    {
        Vector3 forward = _brain.MainCameraTransform.forward; forward.y = 0; forward.Normalize();
        Vector3 right = _brain.MainCameraTransform.right; right.y = 0; right.Normalize();
        return Vector3.one;
    }
}
