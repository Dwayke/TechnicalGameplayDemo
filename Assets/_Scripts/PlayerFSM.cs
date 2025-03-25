using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerFSM:MonoBehaviour
{
    protected BasePlayerState _defaultState;
    public BasePlayerState currentState;
    private PlayerBrain _brain;

    public virtual void Initialize()
    {
        _brain = GetComponent<PlayerBrain>();
    }
    public void UpdateState()
    {
        currentState?.UpdateState();
    }
    public void TransitionToState(BasePlayerState state)
    {
        if (currentState != null)
        {
            currentState?.ExitState();
        }
        currentState = state;
        currentState.EnterState();
    }
    public void SetUpDefaultState()
    {
        //
    }
}
