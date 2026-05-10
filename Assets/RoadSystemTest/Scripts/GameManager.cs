using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public static class TempLevelHolder
{
    public static LevelID nextLevel = LevelID.Pipe;
	public static bool introShown = false;
}

/// <summary>
/// Gestiona el funcionamiento del flujo de bolitas en el juego (game loop)
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }    // Referencia Singleton

    [SerializeField] GameObject[] levels;
    [SerializeField] GameObject blackBackground;

    [HideInInspector] public bool isPlaying = false;            // Controla si el sistema de bolitas está en marcha
    //[HideInInspector] public bool heartPlaced = false;          // Controla si el corazón ha sido colocado

    OrganLogic organLogic;                                      // Referencia al controlador de la lógica del órgano

    public LevelID currentLevel;

    public GameObject currentLevelGameObject;

    public event Action<OrganData, Vector3> OnOrganPlaced;

    public void NotifyOrganPlaced(OrganData organ, Vector3 organPosition)
    {
        OnOrganPlaced?.Invoke(organ, organPosition);
    }

    [SerializeField] LevelOrganMap[] organMappings;
    [SerializeField] TutorialManager tutorialManager;
    [HideInInspector] public bool failed = false;

    [HideInInspector] public int velocityMultiplier = 1;

    [SerializeField] BallMaterialsConfig materialsConfig;

    [SerializeField] MenuPauseController menuPauseController;

    private SessionTimer timer;
    public int tubCol;
    private int intentos;

    private void Awake()
    {
        Instance = this;    // Inicializamos el Singleton

        materialsConfig.RegisterAll();
    }

    private void Start()
    {
        LoadLevel(TempLevelHolder.nextLevel);
    }

    public void LoadLevel(LevelID level)
    {
        timer = new SessionTimer();
        timer.Start();
        tubCol = 0;
        intentos = 0;

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

        if (level == LevelID.Pipe)
        {
            if (!TempLevelHolder.introShown)
            {
                TempLevelHolder.introShown = true;

                blackBackground.SetActive(true);

                DialogManager.instance.Show("dialog_1");
                DialogManager.instance.Show("dialog_2");

                DialogManager.pendingEvents.Enqueue(() =>
                {
                    blackBackground.SetActive(false);

                    // Instanciar el nivel
                    currentLevelGameObject = Instantiate(levels[index]);

                        DialogManager.instance.Show("dialog_3");
                        DialogManager.instance.Show("dialog_4");
                        DialogManager.instance.Show("dialog_5");

                        tutorialManager.ShowTutorial(0);
                });
                return;
            }
        }
        else if (level == LevelID.Heart)
        {
            currentLevelGameObject = Instantiate(levels[index]);
        }

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
        intentos++;
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

        GameManager.Instance.failed = false;
    }

    public void LoadScene(string targetScene)
    {
        SceneManager.LoadScene(targetScene);
    }

    public void EndLevel()
    {
        LevelProgress.CompleteLevel((int)currentLevel);
		if (currentLevel == LevelID.Pipe)
		{
			DialogManager.instance.Show("dialog_5_isgood");
			DialogManager.instance.Show("dialog_6");

            TerminarMinijuego("MinijuegoTuberiasTutorial");

            if (DialogManager.IsDialogActive)
            {
                DialogManager.pendingEvents.Enqueue(() => LoadNextLevel());
                return;
            }
		}

        if (currentLevel == LevelID.Heart)
        {
			DialogManager.instance.Show("dialog_26_isgood");

            LevelProgress.ResetProgress();

            TempLevelHolder.nextLevel = LevelID.Pipe;

            TerminarMinijuego("MinijuegoTuberiasCorazon");

            if (DialogManager.IsDialogActive)
            {
                // Cambiar por -> Llamar a Créditos
                DialogManager.pendingEvents.Enqueue(() => menuPauseController.ReturnToMenu());
                return;
            }
        }
    }

    public OrganType[] GetOrgansForLevel(LevelID level)
    {
        foreach (var map in organMappings)
            if (map.level == level)
                return map.organs;

        return new OrganType[0];
    }

    private void TerminarMinijuego(string nivel)
    {
        int tiempo = timer.Stop();

        GameParametersMDB.Instance.SaveMinigameData(nivel, tiempo, intentos, null, null, tubCol);
    }
}