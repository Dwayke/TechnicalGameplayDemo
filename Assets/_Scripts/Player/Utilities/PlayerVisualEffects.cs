using UnityEngine;
using Unity.Cinemachine;
using DG.Tweening;
// 1. Player Visual Effects Controller
public class PlayerVisualEffects : MonoBehaviour
{
    [Header("Dust Particles")]
    [SerializeField] private ParticleSystem _movementDustParticles;
    [SerializeField] private ParticleSystem _landingDustParticles;
    [SerializeField] private ParticleSystem _dodgeDustParticles;
    [SerializeField] private float _dustEmissionThreshold = 0.5f;

    [Header("Combat Effects")]
    [SerializeField] private ParticleSystem _attackTrailEffect;
    [SerializeField] private ParticleSystem _heavyAttackImpact;
    [SerializeField] private GameObject _weaponTrailRenderer;

    [Header("Movement Effects")]
    [SerializeField] private Transform _playerModel;
    [SerializeField] private float _rotationLagAmount = 0.85f;
    [SerializeField] private float _rotationLagSpeed = 8f;
    [SerializeField] private float _landingSquashAmount = 0.9f;
    [SerializeField] private float _landingSquashDuration = 0.15f;

    [Header("Screen Effects")]
    [SerializeField] private CinemachineVirtualCamera _playerCamera;
    [SerializeField] private CinemachineImpulseSource _cameraShake;
    [SerializeField] private float _lightAttackShake = 0.3f;
    [SerializeField] private float _heavyAttackShake = 0.8f;
    [SerializeField] private float _dodgeShake = 0.2f;

    private PlayerBrain _brain;
    private Vector3 _lastPosition;
    private Vector3 _lastMovementDirection;
    private Quaternion _targetRotation;
    private bool _isGrounded = true;
    private Vector3 _originalScale;

    private void Start()
    {
        _brain = GetComponent<PlayerBrain>();
        _lastPosition = transform.position;
        _originalScale = _playerModel.localScale;
        _targetRotation = transform.rotation;

        // Subscribe to state change events
        if (_brain.FSM != null)
        {
            // We'll implement this when we add state events
        }
    }

    private void Update()
    {
        UpdateMovementEffects();
        UpdateRotationLag();
        UpdateGroundCheck();
    }

    private void UpdateMovementEffects()
    {
        Vector3 currentPosition = transform.position;
        Vector3 movementThisFrame = currentPosition - _lastPosition;
        float movementSpeed = movementThisFrame.magnitude / Time.deltaTime;

        // Handle movement dust particles
        if (_movementDustParticles != null)
        {
            var emission = _movementDustParticles.emission;
            if (movementSpeed > _dustEmissionThreshold && _isGrounded &&
                (_brain.FSM.currentState == _brain.FSM.runState || _brain.FSM.currentState == _brain.FSM.combatStrafeState))
            {
                if (!emission.enabled)
                {
                    emission.enabled = true;
                    _movementDustParticles.Play();
                }

                // Adjust emission rate based on speed
                emission.rateOverTime = Mathf.Lerp(5f, 20f, movementSpeed / 10f);
            }
            else
            {
                emission.enabled = false;
            }
        }

        // Check for direction changes for dust bursts
        Vector3 currentDirection = movementThisFrame.normalized;
        if (movementSpeed > 2f && Vector3.Dot(currentDirection, _lastMovementDirection) < 0.3f && _isGrounded)
        {
            TriggerDirectionChangeDust();
        }

        _lastPosition = currentPosition;
        _lastMovementDirection = currentDirection;
    }

    private void UpdateRotationLag()
    {
        // Create natural rotation lag for more organic feel
        if (_brain.InputHandler.MovementValue != Vector2.zero)
        {
            Vector3 forward = _brain.MainCameraTransform.forward;
            Vector3 right = _brain.MainCameraTransform.right;
            forward.y = 0; forward.Normalize();
            right.y = 0; right.Normalize();

            Vector2 input = _brain.InputHandler.MovementValue;
            Vector3 targetDirection = (forward * input.y + right * input.x).normalized;

            if (targetDirection != Vector3.zero)
            {
                _targetRotation = Quaternion.LookRotation(targetDirection);
            }
        }

        // Apply lag to player model rotation while keeping collision accurate
        if (_playerModel != null)
        {
            Quaternion currentModelRotation = _playerModel.rotation;
            Quaternion laggedRotation = Quaternion.Slerp(currentModelRotation, _targetRotation, _rotationLagSpeed * Time.deltaTime);

            // Apply some additional lag
            laggedRotation = Quaternion.Slerp(laggedRotation, transform.rotation, _rotationLagAmount);
            _playerModel.rotation = laggedRotation;
        }
    }

    private void UpdateGroundCheck()
    {
        bool wasGrounded = _isGrounded;
        _isGrounded = _brain.CharacterController.isGrounded;

        // Landing effect
        if (!wasGrounded && _isGrounded)
        {
            OnLanding();
        }
    }

    // Public methods for states to call
    public void OnLanding()
    {
        // Landing dust
        if (_landingDustParticles != null)
        {
            _landingDustParticles.Play();
        }

        // Landing squash effect
        if (_playerModel != null)
        {
            _playerModel.DOKill();
            _playerModel.localScale = _originalScale;
            _playerModel.DOScale(_originalScale * _landingSquashAmount, _landingSquashDuration / 2f)
                      .OnComplete(() => {
                          _playerModel.DOScale(_originalScale, _landingSquashDuration / 2f);
                      });
        }

        // Camera shake
        TriggerCameraShake(_dodgeShake);
    }

    public void OnDodgeStart()
    {
        // Dodge dust burst
        if (_dodgeDustParticles != null)
        {
            _dodgeDustParticles.Play();
        }

        // Slight camera shake
        TriggerCameraShake(_dodgeShake);

        // Quick scale punch for impact
        if (_playerModel != null)
        {
            _playerModel.DOKill();
            _playerModel.DOPunchScale(Vector3.one * 0.1f, 0.2f, 1, 0.3f);
        }
    }

    public void OnAttackStart(bool isHeavyAttack = false)
    {
        // Enable weapon trail
        if (_weaponTrailRenderer != null)
        {
            _weaponTrailRenderer.SetActive(true);
        }

        // Attack trail particles
        if (_attackTrailEffect != null)
        {
            _attackTrailEffect.Play();
        }

        // Camera shake based on attack type
        float shakeAmount = isHeavyAttack ? _heavyAttackShake : _lightAttackShake;
        TriggerCameraShake(shakeAmount);

        // Slight forward lean for attack impact
        if (_playerModel != null)
        {
            _playerModel.DOKill();
            Vector3 leanAmount = transform.forward * 0.1f;
            _playerModel.DOPunchPosition(leanAmount, 0.3f, 1, 0.5f);
        }
    }

    public void OnAttackHit(bool isHeavyAttack = false)
    {
        // Impact effects
        if (isHeavyAttack && _heavyAttackImpact != null)
        {
            _heavyAttackImpact.Play();
        }

        // Stronger camera shake on hit
        float shakeAmount = isHeavyAttack ? _heavyAttackShake * 1.5f : _lightAttackShake * 1.2f;
        TriggerCameraShake(shakeAmount);

        // Brief hitstop effect
        StartCoroutine(HitStopCoroutine(isHeavyAttack ? 0.1f : 0.05f));
    }

    public void OnAttackEnd()
    {
        // Disable weapon trail
        if (_weaponTrailRenderer != null)
        {
            _weaponTrailRenderer.SetActive(false);
        }

        // Stop attack particles
        if (_attackTrailEffect != null)
        {
            _attackTrailEffect.Stop();
        }
    }

    public void OnCombatEnter()
    {
        // Subtle camera settings change for combat
        if (_playerCamera != null)
        {
            var composer = _playerCamera.GetCinemachineComponent<CinemachineComposer>();
            if (composer != null)
            {
                // Slightly lower camera angle for combat
                DOTween.To(() => composer.m_TrackedObjectOffset.y,
                          y => composer.m_TrackedObjectOffset.y = y,
                          composer.m_TrackedObjectOffset.y - 0.3f, 0.5f);
            }
        }

        // Combat ready particle effect (optional)
        TriggerCombatReadyEffect();
    }

    public void OnCombatExit()
    {
        // Return camera to normal
        if (_playerCamera != null)
        {
            var composer = _playerCamera.GetCinemachineComponent<CinemachineComposer>();
            if (composer != null)
            {
                DOTween.To(() => composer.m_TrackedObjectOffset.y,
                          y => composer.m_TrackedObjectOffset.y = y,
                          composer.m_TrackedObjectOffset.y + 0.3f, 0.8f);
            }
        }
    }

    private void TriggerDirectionChangeDust()
    {
        if (_movementDustParticles != null)
        {
            // Create a burst of particles on direction change
            _movementDustParticles.Emit(15);
        }
    }

    private void TriggerCameraShake(float intensity)
    {
        if (_cameraShake != null)
        {
            _cameraShake.GenerateImpulse(Vector3.one * intensity);
        }
    }

    private void TriggerCombatReadyEffect()
    {
        // Optional: Add a subtle glow or particle effect when entering combat
        // This could be a brief aura around the player
    }

    private System.Collections.IEnumerator HitStopCoroutine(float duration)
    {
        // Brief pause for impact feel
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = originalTimeScale;
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }
}
