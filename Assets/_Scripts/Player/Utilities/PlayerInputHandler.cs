using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour, Controls.IPlayerActions
{
    public Vector2 MovementValue {  get; private set; }
    private Controls _controls;
    private PlayerBrain _brain;
    private void Start()
    {
        _controls = new Controls();
        _brain = GetComponent<PlayerBrain>();
        _controls.Player.SetCallbacks(this);
        _controls.Player.Enable();
    }
    private void OnDestroy()
    {
        _controls.Player.Disable();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        _brain.FSM.SwitchState(_brain.FSM.jumpState);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        _brain.FSM.SwitchState(_brain.FSM.dashState);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementValue = context.ReadValue<Vector2>();
        _brain.FSM.SwitchState(_brain.FSM.runState);
    }
}
