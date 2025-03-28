using UnityEngine;

public abstract class BasePlayerState : IState
{
    protected PlayerBrain _brain;
    public BasePlayerState(PlayerBrain brain) {  _brain = brain; }
    public virtual void EnterState()
    {
        Debug.Log($"Player Entering {this} State");
    }
    public virtual void ExitState()
    {
        Debug.Log($"Player Exiting {this} State");
    }
    public virtual void UpdateState(float deltaTime)
    {

    }
}
