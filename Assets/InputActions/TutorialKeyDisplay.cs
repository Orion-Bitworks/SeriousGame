using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static GamepadIcons;

public class TutorialKeyDisplay : MonoBehaviour
{
    [SerializeField] private InputActionReference action;
    [SerializeField] private int compositePartIndex = 0;
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private Image iconImage;

    [Header("Gamepad Icons")]
    public GamepadIcons xbox;
    public GamepadIcons ps4;
    [SerializeField] private MouseIcons mouseIcons;


    private void Start()
    {
        UpdateKey();
    }

    private void OnEnable()
    {
        UpdateKey();
        InputSystem.onActionChange += OnActionChange;
        DeviceChangeListener.OnDeviceChanged += UpdateKey;
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
        DeviceChangeListener.OnDeviceChanged -= UpdateKey;
    }

    private void OnActionChange(object obj, InputActionChange change)
    {
        if (obj == action.action)
            UpdateKey();
    }

    private void UpdateKey()
    {
        var actionObj = action.action;
        var bindings = actionObj.bindings;

        // Detectar dispositivo activo
        bool usingGamepad = Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame;
        bool usingKeyboard = Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame;

        // Elegir control scheme
        string activeScheme = usingGamepad ? "GamepadControls" : "KeyboardMouseControl";

        for (int i = 0; i < bindings.Count; i++)
        {
            var b = bindings[i];

            if (b.isComposite) continue;
            if (!b.groups.Contains(activeScheme)) continue;

            // Si es composite
            if (b.isPartOfComposite)
            {
                int partIndex = i + compositePartIndex;
                if (partIndex < bindings.Count && bindings[partIndex].isPartOfComposite)
                {
                    HandleDisplay(actionObj, partIndex, usingGamepad);
                    return;
                }
            }
            else
            {
                HandleDisplay(actionObj, i, usingGamepad);
                return;
            }
        }

        // Fallback
        HandleDisplay(actionObj, -1, usingGamepad);
    }

    private void HandleDisplay(InputAction actionObj, int bindingIndex, bool usingGamepad)
    {
        string deviceLayout;
        string controlPath;

        actionObj.GetBindingDisplayString(bindingIndex, out deviceLayout, out controlPath);

        // Normalizar controlPath
        controlPath = controlPath
            .Replace("<Gamepad>/", "")
            .Replace("<DualShockGamepad>/", "")
            .Replace("<DualSenseGamepad>/", "")
            .Replace("<XInputController>/", "")
            .Replace("<Mouse>/", "");

        // Si es mando, icono de mando
        if (usingGamepad)
        {
            textField.gameObject.SetActive(false);
            iconImage.gameObject.SetActive(true);

            Sprite icon = null;

            if (deviceLayout.Contains("DualShock") || deviceLayout.Contains("DualSense"))
                icon = ps4.GetSprite(controlPath);
            else
                icon = xbox.GetSprite(controlPath);

            iconImage.sprite = icon;
            return;
        }

        // Si es ratón, icono de ratón
        if (deviceLayout.Contains("Mouse"))
        {
            Sprite mouseIcon = mouseIcons.GetSprite(controlPath);

            if (mouseIcon != null)
            {
                textField.gameObject.SetActive(false);
                iconImage.gameObject.SetActive(true);
                iconImage.sprite = mouseIcon;
                return;
            }
        }

        // Si es teclado, texto
        iconImage.gameObject.SetActive(false);
        textField.gameObject.SetActive(true);

        string key = bindingIndex >= 0 ?
            actionObj.GetBindingDisplayString(bindingIndex) :
            actionObj.GetBindingDisplayString();

        key = key.Replace("Control", "Ctrl");
        key = key.Replace("Retroceso | Supr", "Supr");
        key = key.Replace("Barra Espaciadora", "Espacio");

        textField.text = key;
    }
}

[Serializable]
public struct GamepadIcons
{
    public Sprite buttonSouth;
    public Sprite buttonNorth;
    public Sprite buttonEast;
    public Sprite buttonWest;
    public Sprite startButton;
    public Sprite selectButton;
    public Sprite leftTrigger;
    public Sprite rightTrigger;
    public Sprite leftShoulder;
    public Sprite rightShoulder;
    public Sprite dpad;
    public Sprite dpadUp;
    public Sprite dpadDown;
    public Sprite dpadLeft;
    public Sprite dpadRight;
    public Sprite leftStickUp;
    public Sprite leftStickDown;
    public Sprite leftStickLeft;
    public Sprite leftStickRight;
    public Sprite rightStickUp;
    public Sprite rightStickDown;
    public Sprite rightStickLeft;
    public Sprite rightStickRight;
    public Sprite leftStickPress;
    public Sprite rightStickPress;

    public Sprite GetSprite(string controlPath)
    {
        switch (controlPath)
        {
            case "buttonSouth": return buttonSouth;
            case "buttonNorth": return buttonNorth;
            case "buttonEast": return buttonEast;
            case "buttonWest": return buttonWest;
            case "start": return startButton;
            case "select": return selectButton;
            case "leftTrigger": return leftTrigger;
            case "rightTrigger": return rightTrigger;
            case "leftShoulder": return leftShoulder;
            case "rightShoulder": return rightShoulder;
            case "dpad": return dpad;
            case "dpad/up": return dpadUp;
            case "dpad/down": return dpadDown;
            case "dpad/left": return dpadLeft;
            case "dpad/right": return dpadRight;
            case "leftStick/up": return leftStickUp;
            case "leftStick/down": return leftStickDown;
            case "leftStick/left": return leftStickLeft;
            case "leftStick/right": return leftStickRight;
            case "rightStick/up": return rightStickUp;
            case "rightStick/down": return rightStickDown;
            case "rightStick/left": return rightStickLeft;
            case "rightStick/right": return rightStickRight;
            case "leftStickPress": return leftStickPress;
            case "rightStickPress": return rightStickPress;
        }
        return null;
    }

    [Serializable]
    public struct MouseIcons
    {
        public Sprite leftButton;
        public Sprite rightButton;
        public Sprite middleButton;

        public Sprite GetSprite(string controlPath)
        {
            switch (controlPath)
            {
                case "leftButton": return leftButton;
                case "rightButton": return rightButton;
                case "middleButton": return middleButton;
            }
            return null;
        }
    }
}