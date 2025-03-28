using Animancer;
using UnityEngine;

public class PlayerIdleState : BasePlayerState
{
    AnimancerState _currentState;
    public PlayerIdleState(PlayerBrain brain) : base(brain)
    {
    }
    public override void EnterState()
    {
        base.EnterState();
        _currentState = _brain.animancer.Play(_brain.animations.playerIdle);
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
