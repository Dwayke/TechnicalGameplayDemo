using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [field: SerializeField] public LayerMask EnemyLayerMask { get; private set; }
    [field: SerializeField] public float CombatDetectionRange { get; private set; } = 8f;

    public bool IsInCombat { get; private set; }
    public bool HasTarget => CurrentTarget != null;
    public Transform CurrentTarget { get; private set; }
    public bool IsInvincible { get; private set; }

    private PlayerBrain _brain;

    private void Start()
    {
        _brain = GetComponent<PlayerBrain>();
    }

    private void Update()
    {
        UpdateCombatState();
        UpdateTargeting();
    }

    private void UpdateCombatState()
    {
        bool enemiesNearby = IsEnemyNearby();

        // Enter combat
        if (!IsInCombat && enemiesNearby)
        {
            EnterCombat();
        }
        // Exit combat handled by individual states with timers
    }

    private void UpdateTargeting()
    {
        if (IsInCombat)
        {
            // Find closest enemy as target
            Transform closestEnemy = FindClosestEnemy();
            CurrentTarget = closestEnemy;
        }
        else
        {
            CurrentTarget = null;
        }
    }

    public void EnterCombat()
    {
        if (IsInCombat) return;

        IsInCombat = true;
        _brain.FSM.EnterCombat();

        // Optional: Trigger combat UI, music, etc.
        OnCombatEntered();
    }

    public void ExitCombat()
    {
        if (!IsInCombat) return;

        IsInCombat = false;
        CurrentTarget = null;
        _brain.FSM.ExitCombat();

        // Optional: Trigger UI changes, music, etc.
        OnCombatExited();
    }

    public void SetInvincible(bool invincible)
    {
        IsInvincible = invincible;

        // Optional: Visual feedback for invincibility
        if (invincible)
            OnInvincibilityStarted();
        else
            OnInvincibilityEnded();
    }

    private bool IsEnemyNearby()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, CombatDetectionRange, EnemyLayerMask);
        return enemies.Length > 0;
    }

    private Transform FindClosestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, CombatDetectionRange * 1.5f, EnemyLayerMask);

        if (enemies.Length == 0) return null;

        Transform closest = null;
        float closestDistance = float.MaxValue;

        foreach (var enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closest = enemy.transform;
                closestDistance = distance;
            }
        }

        return closest;
    }

    // Events for other systems to hook into
    private void OnCombatEntered()
    {
        Debug.Log("Entered Combat Mode");

        // Visual effects
        if (_brain.VisualEffects != null)
        {
            _brain.VisualEffects.OnCombatEnter();
        }

        // Audio effects
        if (_brain.AudioManager != null)
        {
            _brain.AudioManager.PlayCombatEnterSound();
        }
    }

    private void OnCombatExited()
    {
        Debug.Log("Exited Combat Mode");

        // Visual effects
        if (_brain.VisualEffects != null)
        {
            _brain.VisualEffects.OnCombatExit();
        }

        // Audio effects
        if (_brain.AudioManager != null)
        {
            _brain.AudioManager.PlayCombatExitSound();
        }
    }

    private void OnInvincibilityStarted()
    {
        // Visual feedback - flash player, change shader, etc.
    }

    private void OnInvincibilityEnded()
    {
        // Remove visual feedback
    }

    private void OnDrawGizmosSelected()
    {
        // Show combat detection range in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, CombatDetectionRange);

        if (HasTarget)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, CurrentTarget.position);
        }
    }
}