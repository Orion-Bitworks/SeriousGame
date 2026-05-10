using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SFX
{
    Menu = 0,
    Pipe = 1,
    HeartMinigames = 2,
    UI = 3,
    ThreeD = 4,
    Heart = 5,
    TubeAnimation = 6,
    MenuAmbient = 7
}

public enum MenuSFX
{
    Hover = 0,
    Click = 1,
    Tick = 2
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
    Change = 7,
    ParticleInOut = 8
}

public enum HeartMinigamesSFX
{
    Select = 0,
    Rotate = 1,
    Place = 2,
    MonitorStart = 3,
    MonitorShutdown = 4,
    RythmSpawn = 5,
    RythmPerfect = 6,
    RythmCorrect = 7,
    RythmError = 8,
    ScreenInOut = 9
}

public enum UISFX
{
    TableButtons = 0,
    ScreenTouch = 1,
    TutorialClose = 2
}

public enum ThreeDSFX
{
    Select = 0,
    Rotate = 1,
    Place = 2,
    Stretch = 3,
    Pop = 4,
    BloodFlow = 5,
    ScreenCharging = 6,
    ScreenCorrect = 7,
    ScreenError = 8,
    Explosion = 9
}

public enum HeartSFX
{
    ParticleInOut = 0,
    Heartbeat = 1
}

public enum TubeAnimationSFX
{
    TubeIn = 0,
    TubePlaced = 1,
    ParticleMoving = 2
}

public enum MenuAmbientSFX
{
    LightFlicker = 0,
    AirLeak = 1,
    Spark = 2,
    SpotlightOn = 3
}

public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; }

    // Apunta a los eventos del FMOD añadidos
    [Header("Eventos")]
    [SerializeField] FMODUnity.EventReference mainSong;
    [SerializeField] FMODUnity.EventReference ambienceSong;
    [SerializeField] FMODUnity.EventReference creditsSong;
    [SerializeField] FMODUnity.EventReference menuSFX;
    [SerializeField] FMODUnity.EventReference pipeSFX;
    [SerializeField] FMODUnity.EventReference uiSFX;
    [SerializeField] FMODUnity.EventReference heartMinigamesSFX;
    [SerializeField] FMODUnity.EventReference threeDSFX;
    [SerializeField] FMODUnity.EventReference heartSFX;
    [SerializeField] FMODUnity.EventReference tubeAnimationSFX;
    [SerializeField] FMODUnity.EventReference menuAmbientSFX;

    // Variables para las instancias de los eventos añadidos
    private FMOD.Studio.EventInstance mainSongInstance;
    private FMOD.Studio.EventInstance ambienceSongInstance;
    private FMOD.Studio.EventInstance creditsSongInstance;

    private bool tubeParticleSoundPlaying = false;
    private bool heartbeatPlaying = false;
    private FMOD.Studio.EventInstance tubeParticleInstance;
    private FMOD.Studio.EventInstance heartbeatInstance;

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
        creditsSongInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        if (sceneName == "MainMenuGame")
        {
            mainSongInstance.start();
        }
        else if (sceneName == "RoadSystemTest")
        {
            ambienceSongInstance.start();
        }
        else if (sceneName == "CreditsScene")
        {
            creditsSongInstance.start();
        }
    }

    private void Start()
    {
        // Creamos instancias de los eventos
        mainSongInstance = FMODUnity.RuntimeManager.CreateInstance(mainSong);
        ambienceSongInstance = FMODUnity.RuntimeManager.CreateInstance(ambienceSong);
        creditsSongInstance = FMODUnity.RuntimeManager.CreateInstance(creditsSong);

        mainSongInstance.start();
        PlayHeartbeatOnce();
    }

    public FMOD.Studio.EventInstance PlaySFX(SFX sfx, int action)
    {
        FMODUnity.EventReference genericSFX;
        string parameterName = "";

        switch (sfx)
        {
            case SFX.Menu:
                genericSFX = menuSFX;
                parameterName = "Action";
                break;
            case SFX.Pipe:
                genericSFX = pipeSFX;
                parameterName = "PipeActions";
                break;
            case SFX.UI:
                genericSFX = uiSFX;
                parameterName = "UIActions";
                break;
            case SFX.HeartMinigames:
                genericSFX = heartMinigamesSFX;
                parameterName = "HeartMinigamesActions";
                break;
            case SFX.ThreeD:
                genericSFX = threeDSFX;
                parameterName = "3DActions";
                break;
            case SFX.Heart:
                genericSFX = heartSFX;
                parameterName = "HeartActions";
                break;
            case SFX.TubeAnimation:
                genericSFX = tubeAnimationSFX;
                parameterName = "TubeAnimationActions";
                break;
            case SFX.MenuAmbient:
                genericSFX = menuAmbientSFX;
                parameterName = "MenuAmbientActions";
                break;
            default:
                return default;
        }

        var instance = FMODUnity.RuntimeManager.CreateInstance(genericSFX);

        if (!string.IsNullOrEmpty(parameterName))
            instance.setParameterByName(parameterName, action);

        instance.start();

        return instance;
    }

    // Se llama desde un botón, pausa o reanuda la reproducción de la música de fondo.
    public void PauseAndResumeMainSong()
    {
        if (mainSongInstance.getPaused(out bool paused) == FMOD.RESULT.OK)
        {
            mainSongInstance.setPaused(!paused);
        }
    }

    public void PlayTubeParticleOnce()
    {
        if (tubeParticleSoundPlaying)
            return;

        tubeParticleInstance = FMODUnity.RuntimeManager.CreateInstance(tubeAnimationSFX);
        tubeParticleInstance.setParameterByName("TubeAnimationActions", (int)TubeAnimationSFX.ParticleMoving);
        tubeParticleInstance.start();

        tubeParticleSoundPlaying = true;
    }

    public void StopTubeParticle()
    {
        if (!tubeParticleSoundPlaying)
            return;

        tubeParticleInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        tubeParticleInstance.release();
        tubeParticleSoundPlaying = false;
    }

    public void PlayHeartbeatOnce()
    {
        if (heartbeatPlaying)
            return;

        heartbeatInstance = FMODUnity.RuntimeManager.CreateInstance(heartSFX);
        heartbeatInstance.setParameterByName("HeartActions", (int)HeartSFX.Heartbeat);
        heartbeatInstance.start();

        heartbeatPlaying = true;
    }

    public void StopHeartbeat()
    {
        if (!heartbeatPlaying)
            return;

        heartbeatInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        heartbeatInstance.release();
        heartbeatPlaying = false;
    }
}