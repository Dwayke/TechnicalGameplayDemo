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
        Vector3 movement = new()
        {
            x = _brain.InputHandler.MovementValue.x,
            y = 0,
            z = _brain.InputHandler.MovementValue.y
        };
        _brain.CharacterController.Move(_brain.Locomotion.FreeLookMovementSpeed * deltaTime * movement);
        if (_brain.InputHandler.MovementValue == Vector2.zero)
        {
            _brain.FSM.SwitchState(_brain.FSM.idleState);
        }
        _brain.transform.rotation = Quaternion.LookRotation(movement);
    }
    public override void ExitState()
    {
        base.ExitState();
    }
}
