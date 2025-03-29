

using Animancer;

public class PlayerAttackState : BasePlayerState
{
    private AnimancerState _currentState;
    public PlayerAttackState(PlayerBrain brain) : base(brain)
    {
    }
    public override void EnterState()
    {
        base.EnterState();
        _currentState = _brain.Animancer.Play(_brain.Animations.playerAttack);
        _currentState.Events(_brain.Animations.playerAttack).OnEnd = InitiateIdleState;
    }
    public override void UpdateState(float deltaTime)
    {
        base.UpdateState(deltaTime);
    }
    public override void ExitState()
    {
        base.ExitState();
    }

    private void InitiateIdleState()
    {
        _brain.FSM.SwitchState(_brain.FSM.idleState);
    }
}
