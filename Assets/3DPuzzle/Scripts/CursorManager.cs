using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;

public class CursorManager : MonoBehaviour
{
    public static Vector2 Position { get; private set; }

    public static bool VirtualCursorActive { get; private set; }

    [SerializeField] private float gamepadSpeed = 800f;

    [SerializeField] VirtualMouseInput virtualCursor;

    private bool initialized = false;

    public static bool IsGamepadMode { get; private set; } = false;

    private float mouseThreshold = 0.1f;


    private IEnumerator Start()
    {
        while (virtualCursor == null)
        {
            yield return null;
        }

        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;

        VirtualCursorActive = virtualCursor.virtualMouse != null;

        DetectControlMode();

        if (!VirtualCursorActive)
        {
            IsGamepadMode = false;
            MoveWithMouse();
            return;
        }

        if (IsGamepadMode)
        {
            MoveWithGamepad();
        }
        else
        {
            MoveWithMouse();
        }
    }

    private void DetectControlMode()
    {
        // Detectar ratón físico
        if (Mouse.current != null &&
            Mouse.current.delta.ReadValue().sqrMagnitude > mouseThreshold)
        {
            IsGamepadMode = false;
            return;
        }

        // Detectar mando
        if (Gamepad.current != null)
        {
            if (Gamepad.current.dpad.ReadValue().sqrMagnitude > 0.1f ||
                Gamepad.current.leftStick.ReadValue().sqrMagnitude > 0.1f ||
                Gamepad.current.buttonSouth.isPressed ||
                Gamepad.current.buttonNorth.isPressed ||
                Gamepad.current.buttonEast.isPressed ||
                Gamepad.current.buttonWest.isPressed ||
                Gamepad.current.leftShoulder.isPressed ||
                Gamepad.current.rightShoulder.isPressed)
            {
                IsGamepadMode = true;
                return;
            }
        }
    }

    void MoveWithMouse()
    {
        Position = Mouse.current.position.ReadValue();
    }

    void MoveWithGamepad()
    {
        if (!initialized) return;
        if (virtualCursor == null) return;
        if (virtualCursor.virtualMouse == null) return;

        Vector2 move = Gamepad.current.leftStick.ReadValue();

        Vector2 pos = virtualCursor.virtualMouse.position.ReadValue();

        pos += move * gamepadSpeed * Time.unscaledDeltaTime;
        pos = ClampToScreen(pos);

        InputState.Change(virtualCursor.virtualMouse.position, pos);

        Position = pos;
    }

    Vector2 ClampToScreen(Vector2 pos)
    {
        pos.x = Mathf.Clamp(pos.x, 0, Screen.width);
        pos.y = Mathf.Clamp(pos.y, 0, Screen.height);
        return pos;
    }
}