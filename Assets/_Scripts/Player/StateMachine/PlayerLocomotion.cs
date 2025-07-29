using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    private PlayerBrain _brain;

    [field:SerializeField] public float FreeLookMovementSpeed {  get; private set; }
    [field: SerializeField] public float Acceleration { get; private set; } = 10f;
    [field: SerializeField] public float Deceleration { get; private set; } = 15f;
    private Vector3 _currentVelocity;
    [field: SerializeField] public float Gravity { get; private set; } = -9.81f;
    [field: SerializeField] public float AirControl { get; private set; } = 0.3f;
    private float _verticalVelocity;

    private void Awake()
    {
        _brain = GetComponent<PlayerBrain>();
    }

    public Vector3 CalculateVelocity(Vector3 targetDirection, float deltaTime)
    {
        float targetSpeed = targetDirection.magnitude * FreeLookMovementSpeed;
        float acceleration = targetDirection != Vector3.zero ? Acceleration : Deceleration;

        _currentVelocity = Vector3.MoveTowards(_currentVelocity,
            targetDirection * targetSpeed, acceleration * deltaTime);

        return _currentVelocity;
    }

    public void ApplyGravity(float deltaTime)
    {
        if (_brain.CharacterController.isGrounded)
            _verticalVelocity = -2f; // Small negative to stay grounded
        else
            _verticalVelocity += Gravity * deltaTime;
    }

    public float GetMovementSpeedMultiplier(MovementContext context)
    {
        return context switch
        {
            MovementContext.Normal => 1.0f,
            MovementContext.Combat => 0.8f,
            MovementContext.Attacking => 0.3f,
            MovementContext.Dodging => 1.5f,
            MovementContext.Stunned => 0.0f,
            _ => 1.0f
        };
    }
}
public enum MovementContext
{
    Normal,
    Combat,
    Attacking,
    Dodging,
    Stunned
}