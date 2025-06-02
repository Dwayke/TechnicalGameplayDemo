using Animancer;
using UnityEngine;

public class PlayerBrain : MonoBehaviour 
{
    [field:SerializeField]public PlayerFSM FSM {  get; private set; }
    [field:SerializeField]public PlayerLocomotion Locomotion { get; private set; }
    [field:SerializeField]public PlayerInputHandler InputHandler {  get; private set; }
    [field:SerializeField]public CharacterController CharacterController {  get; private set; }
    [field:SerializeField]public AnimancerComponent Animancer {  get; private set; }
    [field:SerializeField]public PlayerAnimations Animations {  get; private set; }

    public Transform MainCameraTransform { get; private set; }
   private void OnEnable()
    {
        FSM = GetComponent<PlayerFSM>();
        Locomotion = GetComponent<PlayerLocomotion>();
        InputHandler = GetComponent<PlayerInputHandler>();
        CharacterController = GetComponent<CharacterController>();
        MainCameraTransform = Camera.main.transform;
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
public class PlayerAnimations
{
    public TransitionAsset playerIdle;
    public TransitionAsset playerRun;
    public TransitionAsset playerAttack;
}
