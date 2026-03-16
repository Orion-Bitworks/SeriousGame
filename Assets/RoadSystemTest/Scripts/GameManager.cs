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
    [HideInInspector] public bool heartPlaced = false;          // Controla si el corazón ha sido colocado

    HeartLogic heartLogic;                                      // Referencia al controlador de la lógica del corazón

    LevelID currentLevel;

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
        if (level == 0)
            FindAnyObjectByType<HeartDrag3D>().DespawnMiniHeart();
        else
            FindAnyObjectByType<HeartDrag3D>().SpawnMiniHeart();

        currentLevel = level;

        int index = (int)level;

        if (level > 0 && !LevelProgress.IsLevelCompleted(index - 1))
        {
            Debug.Log("Nivel bloqueado");
            return;
        }

        Instantiate(levels[index]);
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
        MovingBall[] balls = FindObjectsOfType<MovingBall>();
        foreach (var ball in balls)
            ball.DestroyBall();

        // Desactiva todos los condicionales de las salidas
        RoadOutput[] roadOutputs = FindObjectsOfType<RoadOutput>();
        foreach (var roadOutput in roadOutputs)
            roadOutput.ballReceived = false;

        // Desactiva todos los inputs del sistema
        LevelInputActivator[] levelInputs = FindObjectsOfType<LevelInputActivator>();
        foreach (var levelInput in levelInputs)
            levelInput.DeactivateInputs();

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

    public void EndLevel()
    {
        LevelProgress.CompleteLevel((int)currentLevel);
        gameoverPanel.SetActive(true);

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(LoadNextLevel);
    }
}

