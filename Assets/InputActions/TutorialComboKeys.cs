using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialComboKeys : MonoBehaviour
{
    [SerializeField] private InputActionReference action;
    [SerializeField] private TextMeshProUGUI textField;

    private void OnEnable()
    {
        UpdateKeys();
        InputSystem.onActionChange += OnActionChange;
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
    }

    private void OnActionChange(object obj, InputActionChange change)
    {
        if (obj == action.action)
            UpdateKeys();
    }

    private void UpdateKeys()
    {
        string result = "";

        for (int i = 0; i < action.action.bindings.Count; i++)
        {
            var binding = action.action.bindings[i];

            // Si es un binding compuesto (modifier)
            if (binding.isComposite)
            {
                string composite = "";

                // Leer todas las partes del composite
                int part = i + 1;
                while (part < action.action.bindings.Count && action.action.bindings[part].isPartOfComposite)
                {
                    composite += action.action.GetBindingDisplayString(part) + " + ";
                    part++;
                }

                // Quitar el último " + "
                composite = composite.TrimEnd(' ', '+');

                result = composite;
                break;
            }
        }

        // Si no es composite, usar el binding normal
        if (string.IsNullOrEmpty(result))
            result = action.action.GetBindingDisplayString();

        textField.text = result;
    }
}
