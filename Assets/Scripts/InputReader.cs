using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, Controls.IPlayerActions
{
    #region Public Values
    public Vector2 MovementValue {  get; private set; }
    public bool SprintValue {  get; private set; }
    public bool ADSValue {  get; private set; }
    public bool isShooting {  get; private set; }
    public bool isShootingSemi {  get; private set; }
    public bool isReload {  get; private set; }
    public bool isCover {  get; private set; }
    #endregion

    #region Private Values
    private Controls _controls;
    #endregion

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _controls = new Controls();
        _controls.Player.SetCallbacks(this);

        _controls.Player.Enable();
    }

    private void OnDestroy()
    {
        _controls.Player.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        MovementValue = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        SprintValue = context.performed;
    }

    public void OnADS(InputAction.CallbackContext context)
    {
        ADSValue = context.performed;
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        isShooting = context.performed;
        isShootingSemi =context.started;
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        isReload = context.performed;
    }

    public void OnCover(InputAction.CallbackContext context)
    {
        isCover = context.performed;
    }
}
