using UnityEngine;

public class PlayerFSM : MonoBehaviour
{
    protected BasePlayerState _defaultState;
    public PlayerIdleState idleState;
    public PlayerJumpState jumpState;
    public PlayerDashState dashState;
    public BasePlayerState currentState;
    private PlayerBrain _brain;
    public virtual void Initialize()
    {
        _brain = GetComponent<PlayerBrain>();
        idleState = new PlayerIdleState(_brain);
        jumpState = new PlayerJumpState(_brain);
        dashState = new PlayerDashState(_brain);
        SetUpDefaultState();
        TransitionToState(_defaultState);
    }
    public void UpdateState()
    {
        currentState?.UpdateState(Time.deltaTime);
    }
    public void TransitionToState(BasePlayerState state)
    {
        currentState?.ExitState();
        currentState = state;
        currentState?.EnterState();
    }
    public void SetUpDefaultState()
    {
        _defaultState = idleState;
    }
}
