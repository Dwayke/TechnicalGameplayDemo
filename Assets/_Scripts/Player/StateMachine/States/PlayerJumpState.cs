
using Cysharp.Threading.Tasks;

public class PlayerJumpState : BasePlayerState
{
    public PlayerJumpState(PlayerBrain brain) : base(brain)
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
   
