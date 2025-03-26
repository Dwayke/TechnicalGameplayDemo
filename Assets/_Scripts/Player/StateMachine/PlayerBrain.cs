using Animancer;
using UnityEngine;

public class PlayerBrain : MonoBehaviour 
{
    public PlayerFSM fsm;
    public AnimancerComponent animancer;
    public Animations animations;
    private void OnEnable()
    {
        fsm = GetComponent<PlayerFSM>();
    }
    private void Start()
    {
        fsm.Initialize();
    }
    private void Update()
    {
        fsm.UpdateState();
    }
}
[System.Serializable]
public class Animations
{
    public TransitionAsset playerIdle;
}
