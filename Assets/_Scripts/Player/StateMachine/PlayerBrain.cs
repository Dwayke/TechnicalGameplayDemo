using UnityEngine;

public class PlayerBrain : MonoBehaviour 
{
    public PlayerFSM fsm;

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
