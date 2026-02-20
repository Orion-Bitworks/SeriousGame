using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gestiona el funcionamiento del flujo de bolitas en el juego (game loop)
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }    // Referencia Singleton

    [HideInInspector] public bool isPlaying = false;            // Controla si el sistema de bolitas está en marcha
    [HideInInspector] public bool heartPlaced = false;          // Controla si el corazón ha sido colocado

    HeartLogic heartLogic;                                      // Referencia al controlador de la lógica del corazón

    private void Awake()
    {
        Instance = this;    // Inicializamos el Singleton
    }

    /// <summary>
    /// Pone el sistema de bolitas en marcha
    /// </summary>
    public void Play()
    {
        isPlaying = true;
    }

    /// <summary>
    /// Para el sistema de bolitas
    /// </summary>
    public void Stop()
    {
        isPlaying = false;

        // Busca y elimina todas las bolitas activas
        MovingBall[] balls = FindObjectsOfType<MovingBall>();
        foreach (var ball in balls)
            ball.DestroyBall();

        // Desactiva todos los condicionales de las salidas
        RoadOutput[] roadOutputs = FindObjectsOfType<RoadOutput>();
        foreach (var roadOutput in roadOutputs)
            roadOutput.ballReceived = false;

        // Desactiva todos los inputs del sistema
        HeartLevelInput[] heartLevelInputs = FindObjectsOfType<HeartLevelInput>();
        foreach (var heartLevelInput in heartLevelInputs)
            heartLevelInput.DeactivateInputs();

        // Desactiva los inputs de dentro del sistema del corazón
        if (heartLogic = FindAnyObjectByType<HeartLogic>())
        {
            heartLogic.DeactivatePulmonaryArteries();
            heartLogic.DeactivateAorta();
        }
    }
}

