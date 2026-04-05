using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class TempLevelHolder
{
    public static LevelID nextLevel = LevelID.Pipe;
}

/// <summary>
/// Gestiona el funcionamiento del flujo de bolitas en el juego (game loop)
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }    // Referencia Singleton

    [SerializeField] GameObject gameoverPanel;                  // Referencia al panel de GameOver
    [SerializeField] Button continueButton;                     // Referencia al panel de GameOver
    [SerializeField] GameObject[] levels;

    [HideInInspector] public bool isPlaying = false;            // Controla si el sistema de bolitas está en marcha
    //[HideInInspector] public bool heartPlaced = false;          // Controla si el corazón ha sido colocado

    OrganLogic organLogic;                                      // Referencia al controlador de la lógica del órgano

    LevelID currentLevel;

    public GameObject currentLevelGameObject;

    public event Action<OrganData, Vector3> OnOrganPlaced;

    public void NotifyOrganPlaced(OrganData organ, Vector3 organPosition)
    {
        OnOrganPlaced?.Invoke(organ, organPosition);
    }

    [SerializeField] LevelOrganMap[] organMappings;

    private void Awake()
    {
        Instance = this;    // Inicializamos el Singleton
    }

    private void Start()
    {
        LoadLevel(TempLevelHolder.nextLevel);
    }

    public void LoadLevel(LevelID level)
    {
        currentLevel = level;

        // Obtener los órganos requeridos por este nivel
        OrganType[] organsForLevel = GetOrgansForLevel(level);

        // Encontrar todos los mini-órganos del cajón
        var draggers = FindObjectsOfType<OrganDrag3D>(true);

        // Mostrar solo los órganos que pertenecen al nivel
        foreach (var dragger in draggers)
        {
            // Si el órgano está en la lista del nivel, mostrarlo
            if (System.Array.Exists(organsForLevel, o => o == dragger.organData.organType))
            {
                dragger.SpawnMiniOrgan();
            }
            else
            {
                // Si no pertenece al nivel ocultarlo
                dragger.DespawnMiniOrgan();
            }
        }

        // Comprobación de progreso
        int index = (int)level;

        if (level > 0 && !LevelProgress.IsLevelCompleted(index - 1))
        {
            Debug.Log("Nivel bloqueado");
            return;
        }

        // Instanciar el nivel
        currentLevelGameObject = Instantiate(levels[index]);

        // Si el nivel es el del corazón, arrancar su minijuego 3D
        if (level == LevelID.Heart)
            FindObjectOfType<GameLoopController>().Start3DLevel();
    }

    public void LoadNextLevel()
    {
        int nextIndex = (int)currentLevel + 1;

        if (nextIndex >= levels.Length)
        {
            Debug.Log("No hay más niveles");
            return;
        }

        // Guardamos el siguiente nivel en una variable estática temporal
        TempLevelHolder.nextLevel = (LevelID)nextIndex;

        // Recargamos la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        foreach (var ball in FindObjectsOfType<MovingBall>())
            ball.DestroyBall();

        // Desactiva todos los condicionales de las salidas
        foreach (var roadOutput in FindObjectsOfType<RoadOutput>())
            roadOutput.ballReceived = false;

        // Desactiva todos los inputs del sistema
        foreach (var levelInput in FindObjectsOfType<LevelInputActivator>())
            levelInput.DeactivateInputs();

        // Desactiva los inputs de dentro del sistema del corazón
        if (organLogic = FindAnyObjectByType<OrganLogic>())
            organLogic.ResetOrgan();
    }

    public void LoadScene(string targetScene)
    {
        SceneManager.LoadScene(targetScene);
    }

    public void EndLevel()
    {
        LevelProgress.CompleteLevel((int)currentLevel);
        gameoverPanel.SetActive(true);

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(LoadNextLevel);
    }

    public OrganType[] GetOrgansForLevel(LevelID level)
    {
        foreach (var map in organMappings)
            if (map.level == level)
                return map.organs;

        return new OrganType[0];
    }

}