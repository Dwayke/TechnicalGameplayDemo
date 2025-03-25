using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour, Controls.IPlayerActions
{
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
        _brain.fsm.TransitionToState(_brain.fsm.jumpState);
    }
}
