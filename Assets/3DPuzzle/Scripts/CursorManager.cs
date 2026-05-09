using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;

public class CursorManager : MonoBehaviour
{
    public static Vector2 Position { get; private set; }
    public static bool UsingGamepad { get; private set; }

    [SerializeField] private float gamepadSpeed = 800f;

    [SerializeField] VirtualMouseInput virtualCursor;

    private void Update()
    {
        DetectLastInputDevice();

        if (UsingGamepad)
        {
            MoveWithGamepad();
        }
        else
        {
            MoveWithMouse();
        }
    }

    void MoveWithMouse()
    {
        Position = Mouse.current.position.ReadValue();
    }

    void MoveWithGamepad()
    {
        if (virtualCursor == null) return;

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

    void DetectLastInputDevice()
    {
        if (Mouse.current.delta.ReadValue().sqrMagnitude > 0.1f)
            UsingGamepad = false;

        if (Gamepad.current != null &&
            Gamepad.current.leftStick.ReadValue().sqrMagnitude > 0.1f)
            UsingGamepad = true;
    }
}