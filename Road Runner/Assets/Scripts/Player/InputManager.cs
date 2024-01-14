using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput _playerInput;

    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    public void OnToggleInvetoryInput(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            if (_playerInput.currentActionMap.name == "Player")
            {
                _playerInput.SwitchCurrentActionMap("Inventory");
                UIManager.Instance.SetInventory(true);
                SetCursorLocked(false);
            }
            else
            {
                _playerInput.SwitchCurrentActionMap("Player");
                UIManager.Instance.SetInventory(false);
                SetCursorLocked(true);
            }
        }
    }

    public void OnPauseInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_playerInput.currentActionMap.name == "Player")
            {
                _playerInput.SwitchCurrentActionMap("Paused");
                UIManager.Instance.SetPauseUI(true);
                SetCursorLocked(false);
            }
            else
            {
                _playerInput.SwitchCurrentActionMap("Player");
                UIManager.Instance.SetPauseUI(false);
                SetCursorLocked(true);
            }
        }
    }

    public void OnMapInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_playerInput.currentActionMap.name == "Player")
            {
                _playerInput.SwitchCurrentActionMap("Map");
                UIManager.Instance.SetMapUI(true);
                SetCursorLocked(false);
            }
            else
            {
                _playerInput.SwitchCurrentActionMap("Player");
                UIManager.Instance.SetMapUI(false);
                SetCursorLocked(true);
            }
        }
    }   

    public void OnConsoleInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_playerInput.currentActionMap.name == "Player")
            {
                _playerInput.SwitchCurrentActionMap("Console");
                //UIManager.Instance.SetConsoleUI(true);
                //SetCursorLocked(false);
            }
            else
            {
                _playerInput.SwitchCurrentActionMap("Player");
                //UIManager.Instance.SetConsoleUI(false);
                //SetCursorLocked(true);
            }
        }
    }

    public void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
