using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SFX
{
    Menu = 0,
    Pipe = 1,
    HeartMinigames = 2,
    UI = 3
}

public enum MenuSFX
{
    Hover = 0,
    Click = 1,
    Back = 2,
    Confirm = 3,
    Error = 4
}

public enum PipeSFX
{
    Place = 0,
    Rotate = 1,
    Delete = 2,
    GrabOrgan = 3,
    DropOrgan = 4,
    DrawerOpening = 5,
    DrawerClosing = 6,
    Change = 7
}

public enum UISFX
{
    TableButtons = 0,
    ScreenTouch = 1
}

public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; }

    // Apunta a los eventos del FMOD añadidos
    [Header("Eventos")]
    [SerializeField] FMODUnity.EventReference mainSong;
    [SerializeField] FMODUnity.EventReference ambienceSong;
    [SerializeField] FMODUnity.EventReference menuSFX;
    [SerializeField] FMODUnity.EventReference pipeSFX;
    [SerializeField] FMODUnity.EventReference uiSFX;
    [SerializeField] FMODUnity.EventReference heartMinigamesSFX;

    // Variables para las instancias de los eventos añadidos
    private FMOD.Studio.EventInstance mainSongInstance;
    private FMOD.Studio.EventInstance ambienceSongInstance;
    private FMOD.Studio.EventInstance menuSFXInstance;
    private FMOD.Studio.EventInstance pipeSFXInstance;
    private FMOD.Studio.EventInstance uiSFXInstance;
    private FMOD.Studio.EventInstance heartMinigamesSFXInstance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;

        // Paramos ambas por si acaso
        mainSongInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        ambienceSongInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        if (sceneName == "MainMenuGame")
        {
            mainSongInstance.start();
        }
        else if (sceneName == "RoadSystemTest")
        {
            ambienceSongInstance.start();
        }
    }

    private void Start()
    {
        // Creamos instancias de los eventos
        mainSongInstance = FMODUnity.RuntimeManager.CreateInstance(mainSong);
        ambienceSongInstance = FMODUnity.RuntimeManager.CreateInstance(ambienceSong);
        menuSFXInstance = FMODUnity.RuntimeManager.CreateInstance(menuSFX);
        pipeSFXInstance = FMODUnity.RuntimeManager.CreateInstance(pipeSFX);
        uiSFXInstance = FMODUnity.RuntimeManager.CreateInstance(uiSFX);
        heartMinigamesSFXInstance = FMODUnity.RuntimeManager.CreateInstance(heartMinigamesSFX);

        mainSongInstance.start();
    }

    // Se llama desde un botón, activa un sonido aleatorio del efecto MultiInstrument configurado en FMOD.
    public void PlayRandomSound()
    {
        menuSFXInstance.start();
    }

    public void PlaySFX(SFX sfx, int action)
    {
        FMOD.Studio.EventInstance genericSFXInstance;
        FMODUnity.EventReference genericSFX;
        string parameterName = "";

        switch (sfx)
        {
            case SFX.Menu:
                genericSFXInstance = menuSFXInstance;
                genericSFX = menuSFX;
                parameterName = "Action";
                break;
            case SFX.Pipe:
                genericSFXInstance = pipeSFXInstance;
                genericSFX = pipeSFX;
                parameterName = "PipeActions";
                break;
            case SFX.UI:
                genericSFXInstance = uiSFXInstance;
                genericSFX = uiSFX;
                parameterName = "UIActions";
                break;
            case SFX.HeartMinigames:
                genericSFXInstance = heartMinigamesSFXInstance;
                genericSFX = heartMinigamesSFX;
                parameterName = "HeartMinigameActions";
                break;
            default:
                return;
        }

        if (!genericSFXInstance.isValid())
            genericSFXInstance = FMODUnity.RuntimeManager.CreateInstance(genericSFX);

        genericSFXInstance.setParameterByName(parameterName, action);
        genericSFXInstance.start();
    }

    // Se llama desde un botón, pausa o reanuda la reproducción de la música de fondo.
    public void PauseAndResumeMainSong()
    {
        if (mainSongInstance.getPaused(out bool paused) == FMOD.RESULT.OK)
        {
            mainSongInstance.setPaused(!paused);
        }
    }
}