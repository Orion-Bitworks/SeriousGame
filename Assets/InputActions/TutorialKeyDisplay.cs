using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialKeyDisplay : MonoBehaviour
{
    [SerializeField] private InputActionReference action;
    [SerializeField] private int compositePartIndex = 0;
    [SerializeField] private TextMeshProUGUI textField;



    private void OnEnable()
    {
        UpdateKey();
        InputSystem.onActionChange += OnActionChange;
    }

    private void OnDisable()
    {
        InputSystem.onActionChange -= OnActionChange;
    }

    private void OnActionChange(object obj,InputActionChange change)
    {
        if(obj == action.action)
        {
            UpdateKey();
        }

    }

    private void UpdateKey()
    {
        var bindings = action.action.bindings;

        // Buscar el composite
        for (int i = 0; i < bindings.Count; i++)
        {
            if (bindings[i].isComposite)
            {
                int part = i + 1;
                int partCount = 0;

                // Recorremos las partes del composite
                while (part < bindings.Count && bindings[part].isPartOfComposite)
                {
                    if (partCount == compositePartIndex)
                    {
                        string key = action.action.GetBindingDisplayString(part);

                        // Normalizar nombres
                        // Normalizar nombres
                        key = key.Replace("Control", "Ctrl");
                        key = key.Replace("Left Ctrl", "Ctrl");
                        key = key.Replace("Right Ctrl", "Ctrl");

                        // Variantes de espacio
                        key = key.Replace("Space", "Espacio");
                        key = key.Replace("space", "Espacio");
                        key = key.Replace("Spacebar", "Espacio");
                        key = key.Replace("Barra espaciadora", "Espacio");
                        key = key.Replace("Espaciadora", "Espacio");
                        key = key.Replace("Espacio (teclado)", "Espacio");

                        textField.text = key;
                        return;
                    }

                    part++;
                    partCount++;
                }
            }
        }

        // Si no hay composite, fallback
        textField.text = action.action.GetBindingDisplayString();
    }
}
