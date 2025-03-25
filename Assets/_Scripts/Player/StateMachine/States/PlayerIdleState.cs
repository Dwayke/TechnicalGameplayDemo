using UnityEngine;

public class PlayerIdleState : BasePlayerState
{
    public PlayerIdleState(PlayerBrain brain) : base(brain)
    {
    }
    public override void EnterState()
    {
        base.EnterState();
    }
    public override void UpdateState(float deltaTime)
    {
        base.UpdateState(deltaTime);
    }
    public override void ExitState()
    {
        base.ExitState();
    }
}
