using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class InputController : MonoBehaviour
{
    public PlayerInputControl playerInputControl;

    private void Awake()
    {
        playerInputControl = new PlayerInputControl();
    }

    private void OnEnable()
    {
        playerInputControl.Enable();
    }
    private void OnDisable()
    {
        playerInputControl.Disable();
    }
}
