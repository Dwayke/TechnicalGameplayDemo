using Animancer;
using UnityEngine;

public class PlayerBrain : MonoBehaviour
{
    [field: SerializeField] public PlayerFSM FSM { get; private set; }
    [field: SerializeField] public PlayerLocomotion Locomotion { get; private set; }
    [field: SerializeField] public PlayerInputHandler InputHandler { get; private set; }
    [field: SerializeField] public PlayerCombat Combat { get; private set; }
    [field: SerializeField] public CharacterController CharacterController { get; private set; }
    [field: SerializeField] public AnimancerComponent Animancer { get; private set; }
    [field: SerializeField] public PlayerAnimations Animations { get; private set; }

    // Visual Polish Components
    [field: SerializeField] public PlayerVisualEffects VisualEffects { get; private set; }
    [field: SerializeField] public AttackMagnetism AttackMagnetism { get; private set; }
    [field: SerializeField] public PlayerAudioManager AudioManager { get; private set; }

    public Transform MainCameraTransform { get; private set; }

    private void OnEnable()
    {
        FSM = GetComponent<PlayerFSM>();
        Locomotion = GetComponent<PlayerLocomotion>();
        InputHandler = GetComponent<PlayerInputHandler>();
        Combat = GetComponent<PlayerCombat>();
        CharacterController = GetComponent<CharacterController>();

        // Visual Polish Components
        VisualEffects = GetComponent<PlayerVisualEffects>();
        AttackMagnetism = GetComponent<AttackMagnetism>();
        AudioManager = GetComponent<PlayerAudioManager>();

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
    [Header("Basic Animations")]
    public TransitionAsset playerIdle;
    public TransitionAsset playerRun;
    public AttackAnimationData playerAttack;

    [Header("Combat Animations")]
    public TransitionAsset playerCombatIdle;
    public TransitionAsset playerCombatStrafe;
    public TransitionAsset playerDodge;
    public AttackAnimationData playerAttackMovement;
}

[System.Serializable]
public class AttackAnimationData
{
    public TransitionAsset animation;
    public bool useRootMotion = false;
    public bool isHeavyAttack = false; // For visual/audio effects
    [Range(0f, 1f)]
    public float cancelableFromNormalizedTime = 0.6f;
    public float forwardMomentum = 3f;
    [Range(0f, 1f)]
    public float momentumDuration = 0.6f;
}
