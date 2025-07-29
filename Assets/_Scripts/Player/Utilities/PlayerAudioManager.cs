using UnityEngine;

// 3. Enhanced Audio Manager for Movement
public class PlayerAudioManager : MonoBehaviour
{
    [Header("Movement Audio")]
    [SerializeField] private AudioSource _footstepAudioSource;
    [SerializeField] private AudioClip[] _grassFootsteps;
    [SerializeField] private AudioClip[] _stoneFootsteps;
    [SerializeField] private AudioClip[] _metalFootsteps;
    [SerializeField] private float _footstepInterval = 0.5f;

    [Header("Combat Audio")]
    [SerializeField] private AudioSource _combatAudioSource;
    [SerializeField] private AudioClip _attackWhoosh;
    [SerializeField] private AudioClip _heavyAttackWhoosh;
    [SerializeField] private AudioClip _dodgeWhoosh;
    [SerializeField] private AudioClip _landingSound;

    [Header("UI Audio")]
    [SerializeField] private AudioClip _combatEnterStinger;
    [SerializeField] private AudioClip _combatExitStinger;

    private PlayerBrain _brain;
    private float _footstepTimer;
    private GroundType _currentGroundType = GroundType.Grass;

    public enum GroundType { Grass, Stone, Metal }

    private void Start()
    {
        _brain = GetComponent<PlayerBrain>();
    }

    private void Update()
    {
        HandleFootstepAudio();
        DetectGroundType();
    }

    private void HandleFootstepAudio()
    {
        bool isMoving = _brain.InputHandler.MovementValue != Vector2.zero;
        bool isGrounded = _brain.CharacterController.isGrounded;
        bool isInMovementState = _brain.FSM.currentState == _brain.FSM.runState ||
                                _brain.FSM.currentState == _brain.FSM.combatStrafeState;

        if (isMoving && isGrounded && isInMovementState)
        {
            _footstepTimer += Time.deltaTime;
            if (_footstepTimer >= _footstepInterval)
            {
                PlayFootstepSound();
                _footstepTimer = 0f;
            }
        }
        else
        {
            _footstepTimer = 0f;
        }
    }

    private void PlayFootstepSound()
    {
        AudioClip[] footstepClips = _currentGroundType switch
        {
            GroundType.Grass => _grassFootsteps,
            GroundType.Stone => _stoneFootsteps,
            GroundType.Metal => _metalFootsteps,
            _ => _grassFootsteps
        };

        if (footstepClips != null && footstepClips.Length > 0)
        {
            AudioClip clipToPlay = footstepClips[UnityEngine.Random.Range(0, footstepClips.Length)];
            _footstepAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            _footstepAudioSource.PlayOneShot(clipToPlay);
        }
    }

    private void DetectGroundType()
    {
        // Simple ground detection - you can expand this with raycasting and surface detection
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2f))
        {
            if (hit.collider.CompareTag("StoneGround"))
                _currentGroundType = GroundType.Stone;
            else if (hit.collider.CompareTag("MetalGround"))
                _currentGroundType = GroundType.Metal;
            else
                _currentGroundType = GroundType.Grass;
        }
    }

    // Public methods for states to call
    public void PlayAttackSound(bool isHeavyAttack = false)
    {
        AudioClip clipToPlay = isHeavyAttack ? _heavyAttackWhoosh : _attackWhoosh;
        if (clipToPlay != null)
        {
            _combatAudioSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
            _combatAudioSource.PlayOneShot(clipToPlay);
        }
    }

    public void PlayDodgeSound()
    {
        if (_dodgeWhoosh != null)
        {
            _combatAudioSource.PlayOneShot(_dodgeWhoosh);
        }
    }

    public void PlayLandingSound()
    {
        if (_landingSound != null)
        {
            _footstepAudioSource.PlayOneShot(_landingSound);
        }
    }

    public void PlayCombatEnterSound()
    {
        if (_combatEnterStinger != null)
        {
            _combatAudioSource.PlayOneShot(_combatEnterStinger);
        }
    }

    public void PlayCombatExitSound()
    {
        if (_combatExitStinger != null)
        {
            _combatAudioSource.PlayOneShot(_combatExitStinger);
        }
    }
}