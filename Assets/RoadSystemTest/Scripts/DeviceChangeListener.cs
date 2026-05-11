using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class DeviceChangeListener : MonoBehaviour
{
    public static event Action OnDeviceChanged;

    // Estado global del dispositivo activo
    public static string CurrentDevice = "Keyboard";

    private string lastDevice = "Keyboard";

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void Update()
    {
        string currentDevice = "";

        // Detectar actividad de mando
        if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
            currentDevice = "Gamepad";

        // Detectar actividad de teclado o ratón
        else if (Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame)
            currentDevice = "Keyboard";
        else if (Mouse.current != null && Mouse.current.wasUpdatedThisFrame)
            currentDevice = "Keyboard";

        // Si hay cambio real de dispositivo, notificar
        if (currentDevice != "" && currentDevice != lastDevice)
        {
            lastDevice = currentDevice;
            CurrentDevice = currentDevice;
            OnDeviceChanged?.Invoke();
        }
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        // Si se desconecta un mando, volver a teclado
        if (device is Gamepad &&
            (change == InputDeviceChange.Removed || change == InputDeviceChange.Disconnected))
        {
            lastDevice = "Keyboard";
            CurrentDevice = "Keyboard";
            OnDeviceChanged?.Invoke();
        }
    }
}
