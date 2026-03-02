using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gestiona el funcionamiento del flujo de bolitas en el juego (game loop)
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }    // Referencia Singleton

    [SerializeField] GameObject gameoverPanel;                  // Referencia al panel de GameOver

    [HideInInspector] public bool isPlaying = false;            // Controla si el sistema de bolitas está en marcha
    [HideInInspector] public bool heartPlaced = false;          // Controla si el corazón ha sido colocado

    int requiredFinalOutputs = 3;                               // Número mínimo de salidas que deben haber recibido bolitas
    HeartLogic heartLogic;                                      // Referencia al controlador de la lógica del corazón

    private void Awake()
    {
        Instance = this;    // Inicializamos el Singleton
    }

    // Comprueba si el sistema ha sido completado correctamente, y si es así, muestra la pantalla de fin de juego
    private void Update()
    {
        if (CheckFinalSystem())
            gameoverPanel.SetActive(true);
    }

    /// <summary>
    /// Comprueba si todos los outputs del corazón ("requiredFinalOutputs") necesarios han recibido bolitas
    /// </summary>
    /// <returns>True si el número de "FinalHeartOutputs" que han recibido bola es mayor o igual que "requiredFinalOutputs"</returns>
    public bool CheckFinalSystem()
    {
        // Recorremos todos los outputs y sumamos 1 en un contador si los "FinalHeartOutput" han recibido bolita
        RoadOutput[] roadOutputs = FindObjectsOfType<RoadOutput>();
        int count = 0;
        foreach (var roadOutput in roadOutputs)
        {
            if (roadOutput.CompareTag("FinalHeartOutput") && roadOutput.ballReceived)
                count++;
        }

        return count >= requiredFinalOutputs;
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

    public void LoadScene(string targetScene)
    {
        SceneManager.LoadScene(targetScene);
    }
}

