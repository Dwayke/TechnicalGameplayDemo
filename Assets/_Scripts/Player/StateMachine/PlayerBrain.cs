using Animancer;
using UnityEngine;

public class PlayerBrain : MonoBehaviour 
{
    [field:SerializeField]public PlayerFSM FSM {  get; private set; }
    [field:SerializeField]public PlayerLocomotion Locomotion { get; private set; }
    [field:SerializeField]public PlayerInputHandler InputHandler {  get; private set; }
    [field:SerializeField]public CharacterController CharacterController {  get; private set; }
    [field:SerializeField]public AnimancerComponent Animancer {  get; private set; }
    [field:SerializeField]public Animations Animations {  get; private set; }
   private void OnEnable()
    {
        FSM = GetComponent<PlayerFSM>();
        Locomotion = GetComponent<PlayerLocomotion>();
        InputHandler = GetComponent<PlayerInputHandler>();
        CharacterController = GetComponent<CharacterController>();
    }
    private void Start()
    {
        FSM.Initialize();
    }
    private void Update()
    {
        FSM.UpdateState();
    }
}
[System.Serializable]
public class Animations
{
    public TransitionAsset playerIdle;
    public TransitionAsset playerRun;
}
