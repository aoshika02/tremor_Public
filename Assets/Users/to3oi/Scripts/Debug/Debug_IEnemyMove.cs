using UnityEngine;
using UnityEngine.InputSystem;

public class Debug_IEnemyMove : IEnemyMove
{
    private void Start()
    {
        InputManager.Instance.SetRunStartedAction(DebugButton);
    }

    private void DebugButton(InputAction.CallbackContext obj)
    {
        Debug.Log(GetNextPosition().name);
    }
}
