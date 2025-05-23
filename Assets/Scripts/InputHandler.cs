using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputHandler : MonoBehaviour
{
    public PlayerController player;

    private InputAction _moveAction, _lookAction, _jumpAction, _attackAction, _kickAction;

    private void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _jumpAction.performed += OnPlayerJump;

        _attackAction = InputSystem.actions.FindAction("Attack");
        _attackAction.performed += OnPlayerAttack;

        _kickAction = InputSystem.actions.FindAction("Kick");
        _kickAction.performed += OnPlayerKickAttack;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnPlayerKickAttack(InputAction.CallbackContext context)
    {
        player.Attack("Kick");
    }

    private void OnPlayerAttack(InputAction.CallbackContext context)
    {
        player.Attack("Attack");
    }

    private void OnPlayerJump(InputAction.CallbackContext context)
    {
        player.Jump();
    }

    private void Update()
    {
        Vector2 moveVector = _moveAction.ReadValue<Vector2>();
        player.Move(moveVector);
        
        Vector2 lookVector = _lookAction.ReadValue<Vector2>();
        player.Rotate(lookVector);
    }
}
