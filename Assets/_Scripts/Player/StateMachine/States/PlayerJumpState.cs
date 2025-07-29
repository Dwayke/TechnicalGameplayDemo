using Cysharp.Threading.Tasks;

public class PlayerJumpState : BasePlayerState
{
    public PlayerJumpState(PlayerBrain brain) : base(brain)
    {
    }

    public override async void EnterState()
    {
        base.EnterState();

        // Visual effects for jump start (could add jump particles here)

        await UniTask.Delay(2000);

        // Landing effects
        if (_brain.VisualEffects != null)
        {
            _brain.VisualEffects.OnLanding();
        }

        if (_brain.AudioManager != null)
        {
            _brain.AudioManager.PlayLandingSound();
        }

        _brain.FSM.SwitchState(_brain.FSM.idleState);
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