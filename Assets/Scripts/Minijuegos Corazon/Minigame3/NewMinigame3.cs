using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public enum InputType { Keyboard, GamepadButton }

[System.Serializable]
public struct ExpectedInput
{
    public InputType type;
    public KeyCode key;
    public GamepadButton button;
}

public class NewMinigame3 : MonoBehaviour
{
	[Header("Ritmo")]
	public float bpm = 110f;
	private float spawnInterval;
	private float nextSpawnTime;

	[Header("Notas")]
	public GameObject notePrefab;
	public Transform notesPanel;
	private RythmNoteUI activeNote;

	[Header("Gameplay")]
	public int maxNotes = 10;
	private int spawnedNotes = 0;
	private int completedNotes = 0;
	private int fallos = 0;
	private bool gameActive = false;

	[Header("UI")]
	[SerializeField] private GameObject rebootButton;

	private SessionTimer timer;
	private int intentos = 1;

	[SerializeField] private TutorialManager tutorial;

    private bool lastInputWasGamepad = false;

    public event System.Action<bool> OnGameCompleted;

	private bool _gameCompleted = false;
	public bool gameCompleted
	{
		get => _gameCompleted;
		set
		{
			if (_gameCompleted == value)
				return;

			_gameCompleted = value;
			OnGameCompleted?.Invoke(_gameCompleted);
		}
	}

    private void Awake()
    {
        StartCoroutine(ShowInstructions()); //Corrutina per mostrar les instruccions del minijoc
    }

    private void Start()
	{
		rebootButton.SetActive(false);

		timer = new SessionTimer();
		timer.Start();

		spawnInterval = 60f / bpm;
		nextSpawnTime = Time.time + spawnInterval;
	}

	void StartMiniGame3()
	{
		gameActive = true;
	}

	private void Update()
	{
		if (!gameActive) return;

        DetectLastInput();

        if (activeNote == null && Time.time >= nextSpawnTime && spawnedNotes < maxNotes)
		{
			SpawnNote();
			nextSpawnTime = Time.time + spawnInterval;
		}

		if (activeNote != null)
		{
			CheckInput();
		}
	}

	private void SpawnNote()
	{
		spawnedNotes++;

		GameObject obj = Instantiate(notePrefab, notesPanel);
        AudioController.Instance.PlaySFX(SFX.HeartMinigames, (int)HeartMinigamesSFX.RythmSpawn);
        ExpectedInput input = GetRandomInput();
        
        activeNote = obj.GetComponent<RythmNoteUI>();
        activeNote.Init(input, this);

        // POSICIÓN ALEATORIA CONTROLADA
        RectTransform rt = obj.GetComponent<RectTransform>();
		RectTransform panelRT = notesPanel.GetComponent<RectTransform>();

		float maxX = panelRT.rect.width / 2f - rt.rect.width / 2f;
		float maxY = panelRT.rect.height / 2f - rt.rect.height / 2f;

		rt.anchoredPosition = new Vector2(
			Random.Range(-maxX, maxX),
			Random.Range(-maxY, maxY)
		);
	}

    /*private void CheckInput()
	{
		if (activeNote == null) return;

		if (Input.anyKeyDown)
		{
			if (Input.GetKeyDown(activeNote.expectedKey))
			{
				HitResult result = activeNote.GetHitResult();

				if (result == HitResult.Perfect || result == HitResult.Good)
				{
					completedNotes++;
				}
				else
				{
					fallos++;
				}

				activeNote.ShowFeedback(result);
				Destroy(activeNote.gameObject, 0.3f);
				activeNote = null;
			}
			else
			{
				fallos++;
				activeNote.ShowFeedback(HitResult.Miss);
				Destroy(activeNote.gameObject, 0.3f);
				activeNote = null;
			}
		}

		if (completedNotes + fallos >= maxNotes)
		{
			EndMinigame();
		}
	}*/

    private void CheckInput()
    {
        if (activeNote == null) return;

        if (CheckKeyboardInput() || CheckGamepadButton())
        {
            HitResult result = activeNote.GetHitResult();

            if (result == HitResult.Perfect || result == HitResult.Good)
                completedNotes++;
            else
                fallos++;

            activeNote.ShowFeedback(result);
            Destroy(activeNote.gameObject, 0.3f);
            activeNote = null;
        }

        if (completedNotes + fallos >= maxNotes)
            EndMinigame();
    }

    bool CheckKeyboardInput()
    {
        if (activeNote.expectedInput.type != InputType.Keyboard)
            return false;

        return Input.GetKeyDown(activeNote.expectedInput.key);
    }

    bool CheckGamepadButton()
    {
        if (activeNote.expectedInput.type != InputType.GamepadButton)
            return false;

        if (Gamepad.current == null)
            return false;

        return Gamepad.current[activeNote.expectedInput.button].wasPressedThisFrame;
    }

    public void RegisterMiss(RythmNoteUI note)
	{
		if (note != activeNote) return;

		fallos++;
		note.ShowFeedback(HitResult.Miss);
		Destroy(note.gameObject, 0.3f);
		activeNote = null;

		if (completedNotes + fallos >= maxNotes)
			EndMinigame();
	}

	private void EndMinigame()
	{
		gameActive = false;
		int tiempo = timer.Stop();

		if (fallos > 0)
		{
			rebootButton.SetActive(true);
			gameCompleted = false;
		}
		else
		{
			gameCompleted = true;
		}

		GameParametersMDB.Instance.SaveMinigameData(
			"MinijuegoCorazon3",
			tiempo,
			intentos,
			null,
			fallos
		);
	}

	public void RestartMinigame()
	{
		rebootButton.SetActive(false);

		intentos++;
		spawnedNotes = 0;
		completedNotes = 0;
		fallos = 0;

		if (activeNote != null)
			Destroy(activeNote.gameObject);

		activeNote = null;

		timer = new SessionTimer();
		timer.Start();

		nextSpawnTime = Time.time + spawnInterval;
		gameActive = true;
	}

	private KeyCode GetRandomKey()
	{
		KeyCode[] keys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
		return keys[Random.Range(0, keys.Length)];
	}

    private ExpectedInput GetRandomInput()
    {
        ExpectedInput input = new ExpectedInput();

        if (lastInputWasGamepad && Gamepad.current != null)
        {
            input.type = InputType.GamepadButton;
            GamepadButton[] buttons =
            {
            GamepadButton.South,
            GamepadButton.East,
            GamepadButton.West,
            GamepadButton.North
        };
            input.button = buttons[Random.Range(0, buttons.Length)];
        }
        else
        {
            input.type = InputType.Keyboard;
            KeyCode[] keys = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
            input.key = keys[Random.Range(0, keys.Length)];
        }

        return input;
    }

    IEnumerator ShowInstructions()
    {
        gameActive = false;
        Time.timeScale = 0f;

        bool tutorialClosed = false;

        void Handler()
        {
            tutorialClosed = true;
        }

        tutorial.OnTutorialClosed += Handler;

        tutorial.ShowTutorial(4);

        // Esperar hasta que el tutorial se cierre
        yield return new WaitUntil(() => tutorialClosed);

        tutorial.OnTutorialClosed -= Handler;

        Time.timeScale = 1f;
        gameActive = true;

        StartMiniGame3();
    }

    private void DetectLastInput()
    {
        // Si hay mando conectado
        if (Gamepad.current != null)
        {
            if (Gamepad.current.buttonSouth.wasPressedThisFrame ||
                Gamepad.current.buttonNorth.wasPressedThisFrame ||
                Gamepad.current.buttonEast.wasPressedThisFrame ||
                Gamepad.current.buttonWest.wasPressedThisFrame ||
                Gamepad.current.dpad.ReadValue().sqrMagnitude > 0.1f ||
                Gamepad.current.leftStick.ReadValue().sqrMagnitude > 0.1f)
            {
                lastInputWasGamepad = true;
                return;
            }
        }

        // Si se pulsa una tecla
        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            lastInputWasGamepad = false;
        }
    }
}