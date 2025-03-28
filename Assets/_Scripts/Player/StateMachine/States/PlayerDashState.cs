using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerDashState : BasePlayerState
{
    public PlayerDashState(PlayerBrain brain) : base(brain)
    {
    }
    public override async void EnterState()
    {
        base.EnterState();
        await UniTask.Delay(2000);
        _brain.fsm.TransitionToState(_brain.fsm.idleState);
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
