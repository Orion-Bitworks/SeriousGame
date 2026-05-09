using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialKeyDisplay : MonoBehaviour
{
    [SerializeField] private InputActionReference action;
    [SerializeField] private int compositePartIndex = 0;
    [SerializeField] private TextMeshProUGUI textField;

    private void Start()
    {
        UpdateKey();
    }

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
                        string keyComposite = action.action.GetBindingDisplayString(part);

                        // Normalizar nombres compuestos
                        keyComposite = keyComposite.Replace("Control", "Ctrl");

                        textField.text = keyComposite;
                        return;
                    }

                    part++;
                    partCount++;
                }
            }
        }

        string fallback = action.action.GetBindingDisplayString();

        // Normalizar nombres
        fallback = fallback.Replace("Retroceso | Supr", "Supr");
        fallback = fallback.Replace("Barra Espaciadora", "Espacio");

        textField.text = fallback;
    }
}
