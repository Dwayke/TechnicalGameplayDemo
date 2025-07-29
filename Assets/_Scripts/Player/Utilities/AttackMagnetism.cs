using UnityEngine;
// 2. Enemy Magnetism System for attacks
public class AttackMagnetism : MonoBehaviour
{
    [Header("Magnetism Settings")]
    [SerializeField] private float _magnetismRange = 3f;
    [SerializeField] private float _magnetismStrength = 2f;
    [SerializeField] private LayerMask _enemyLayerMask;
    [SerializeField] private bool _enableMagnetism = true;

    private PlayerBrain _brain;

    private void Start()
    {
        _brain = GetComponent<PlayerBrain>();
    }

    public Vector3 ApplyAttackMagnetism(Vector3 originalMovement)
    {
        if (!_enableMagnetism || _brain.FSM.currentState != _brain.FSM.attackState &&
            _brain.FSM.currentState != _brain.FSM.attackMovementState)
        {
            return originalMovement;
        }

        // Find nearest enemy
        Collider nearestEnemy = FindNearestEnemy();
        if (nearestEnemy == null)
        {
            return originalMovement;
        }

        // Calculate magnetism vector
        Vector3 toEnemy = nearestEnemy.transform.position - transform.position;
        toEnemy.y = 0; // Keep horizontal
        float distance = toEnemy.magnitude;

        if (distance > _magnetismRange)
        {
            return originalMovement;
        }

        // Apply magnetism based on distance (stronger when closer)
        float magnetismFactor = 1f - (distance / _magnetismRange);
        Vector3 magnetismVector = toEnemy.normalized * _magnetismStrength * magnetismFactor;

        // Blend with original movement
        return Vector3.Lerp(originalMovement, magnetismVector, 0.3f);
    }

    private Collider FindNearestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, _magnetismRange, _enemyLayerMask);

        if (enemies.Length == 0)
            return null;

        Collider nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearest = enemy;
                nearestDistance = distance;
            }
        }

        return nearest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _magnetismRange);
    }
}
