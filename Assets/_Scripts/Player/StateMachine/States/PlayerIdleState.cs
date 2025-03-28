using Animancer;
using UnityEngine;

public class PlayerIdleState : BasePlayerState
{
    public PlayerIdleState(PlayerBrain brain) : base(brain)
    {
    }
    public override void EnterState()
    {
        base.EnterState();
        _brain.Animancer.Play(_brain.Animations.playerIdle);
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
