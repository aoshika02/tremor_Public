using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class MoveManager : SingletonMonoBehaviour<MoveManager>
{
    [SerializeField] private PlayerInput input;

    // Start is called before the first frame update
    void Start()
    {
        
    }

   
    #region Move

    public void SetMoveStartedAction(Action<InputAction.CallbackContext> action)
    {
        input.actions["Move"].started += action;
    }
    public void SetMovePerformedAction(Action<InputAction.CallbackContext> action)
    {
        input.actions["Move"].performed += action;
    }
    public void SetMoveCanceledAction(Action<InputAction.CallbackContext> action)
    {
        input.actions["Move"].canceled += action;
    }
    public void RemoveMoveStartedAction(Action<InputAction.CallbackContext> action)
    {
        input.actions["Move"].started -= action;
    }
    public void RemoveMovePerformedAction(Action<InputAction.CallbackContext> action)
    {
        input.actions["Move"].performed -= action;
    }
    public void RemoveMoveCanceledAction(Action<InputAction.CallbackContext> action)
    {
        input.actions["Move"].canceled -= action;
    }
    #endregion

    #region Camera
    public void SetCameraStartedAction(Action<InputAction.CallbackContext> action)
    {
        input.actions["Camera"].started += action;
    }
    public void SetCameraPerformedAction(Action<InputAction.CallbackContext> action)
    {
        input.actions["Camera"].performed += action;
    }
    public void SetCameraCanceledAction(Action<InputAction.CallbackContext> action)
    {
        input.actions["Camera"].canceled += action;
    }
    public void RemoveCameraStartedAction(Action<InputAction.CallbackContext> action)
    {
        input.actions["Camera"].started -= action;
    }
    public void RemoveCameraPerformedAction(Action<InputAction.CallbackContext> action)
    {
        input.actions["Camera"].performed -= action;
    }
    public void RemoveCameraCanceledAction(Action<InputAction.CallbackContext> action)
    {
        input.actions["Camera"].canceled -= action;
    }
    #endregion
}
