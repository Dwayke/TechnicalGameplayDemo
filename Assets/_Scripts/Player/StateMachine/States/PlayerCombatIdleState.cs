using UnityEngine;
using Cysharp.Threading.Tasks;

// 4. Combat Idle State - Standing ready in combat
public class PlayerCombatIdleState : BasePlayerState
{
    private float _combatTimer;
    private const float COMBAT_TIMEOUT = 4f;

    public PlayerCombatIdleState(PlayerBrain brain) : base(brain)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        _brain.Animancer.Play(_brain.Animations.playerCombatIdle);
        _combatTimer = 0f;
    }

    public override void UpdateState(float deltaTime)
    {
        base.UpdateState(deltaTime);

        _combatTimer += deltaTime;

        // Auto-exit combat after timeout
        if (!IsNearEnemies() && _combatTimer > COMBAT_TIMEOUT)
        {
            _brain.Combat.ExitCombat();
            _brain.FSM.SwitchState(_brain.FSM.idleState);
            return;
        }

        // Face nearest enemy
        if (_brain.Combat.HasTarget)
        {
            Vector3 lookDirection = (_brain.Combat.CurrentTarget.position - _brain.transform.position);
            lookDirection.y = 0;
            lookDirection.Normalize();

            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                _brain.transform.rotation = Quaternion.Slerp(_brain.transform.rotation, targetRotation, 6f * deltaTime);
            }
        }

        // Handle input transitions
        if (_brain.InputHandler.MovementValue != Vector2.zero)
        {
            _brain.FSM.SwitchState(_brain.FSM.combatStrafeState);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private bool IsNearEnemies()
    {
        Collider[] enemies = Physics.OverlapSphere(_brain.transform.position, 8f, _brain.Combat.EnemyLayerMask);
        return enemies.Length > 0;
    }
}