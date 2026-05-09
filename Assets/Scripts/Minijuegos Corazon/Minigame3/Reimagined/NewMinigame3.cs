using UnityEngine;
using UnityEngine.UI;

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
	[SerializeField] private GameObject[] elementsToHide;

	private SessionTimer timer;
	private int intentos = 1;

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

	private void Start()
	{
		foreach (var obj in elementsToHide)
			obj.SetActive(false);

		rebootButton.SetActive(false);

		timer = new SessionTimer();
		timer.Start();

		spawnInterval = 60f / bpm;
		nextSpawnTime = Time.time + spawnInterval;

		gameActive = true;
	}

	private void Update()
	{
		if (!gameActive) return;

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
		KeyCode key = GetRandomKey();

		activeNote = obj.GetComponent<RythmNoteUI>();
		activeNote.Init(key, this);

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

	private void CheckInput()
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
			foreach (var obj in elementsToHide)
				obj.SetActive(true);

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

		foreach (var obj in elementsToHide)
			obj.SetActive(false);

		timer = new SessionTimer();
		timer.Start();

		nextSpawnTime = Time.time + spawnInterval;
		gameActive = true;
	}

	private KeyCode GetRandomKey()
	{
		KeyCode[] keys = { KeyCode.A, KeyCode.S, KeyCode.D };
		return keys[Random.Range(0, keys.Length)];
	}
}
