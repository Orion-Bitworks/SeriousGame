using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestiona la generación de bolita en los inputs (entradas a venas pulmonares) del nivel del corazón 
/// </summary>
public class HeartLevelInput : MonoBehaviour
{
    RoadInput inputToActivate;      // Referencia al input a activar
    int requiredOutputs = 2;        // Número mínimo de salidas que deben haber recibido bolitas
    bool activated = false;         // Flag para saber si el input ya ha sido activado

    // Nos aseguramos que no se están generando bolitas del input al iniciar
    private void Start()
    {
        inputToActivate = gameObject.GetComponent<RoadInput>();
        inputToActivate.StopGenerating();
    }

    // Si todavia no se ha activado y el sistema da la señal, empezamos a generar bolitas y marcamos el input como activado
    private void Update()
    {
        if (!activated && CheckSystem())
        {
            ActivateInputs();
            activated = true;
        }
    }

    /// <summary>
    /// Comprueba si todos los outputs del corazón ("requiredOutputs") necesarios han recibido bolitas
    /// </summary>
    /// <returns>True si el número de "HeartOutputs" que han recibido bola es mayor o igual que "requiredOutputs"</returns>
    public bool CheckSystem()
    {
        // Recorremos todos los outputs y sumamos 1 en un contador si los "HeartOutput" han recibido bolita
        RoadOutput[] roadOutputs = FindObjectsOfType<RoadOutput>();
        int count = 0;
        foreach (var roadOutput in roadOutputs)
        {
            if (roadOutput.CompareTag("HeartOutput") && roadOutput.ballReceived)
                count++;
        }

        return count >= requiredOutputs;
    }

    /// <summary>
    /// Empieza a generar bolitas
    /// </summary>
    void ActivateInputs()
    {
        inputToActivate.StartGenerating();
    }

    /// <summary>
    /// Detiene la generación de bolitas y marcamos el input como desactivado
    /// </summary>
    public void DeactivateInputs()
    {
        activated = false;
        inputToActivate.StopGenerating();
    }
}
