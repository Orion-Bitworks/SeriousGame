using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Minigame3 : MonoBehaviour
{
    public float bpm; // beats per minute
    private float spawnInterval; //interval de spawn entre nota i nota

    public int maxNotes ; //Quantitat de notes que es mostraran
    private int spawnedNotes = 0;

    public GameObject notePrefab; //prefab de la nota

    //Zona de spawn de les notes
    public Vector3 minSpawn;
    public Vector3 maxSpawn;

    private Queue<RythmNote> noteQueue = new Queue<RythmNote>();
    private RythmNote currentNote;

    private int completedNotes = 0; //notes completades

    private bool gameActive = false; //Variable per pausar el joc

    private List<RythmNote> activateNotes = new List<RythmNote>(); //lista per controlar les notes actives

    [SerializeField] public GameObject rebootButton; //boto per reiniciar el minijoc en cas de haver-ho fet malament

    //ELEMENTOS A ESCONDER
    [SerializeField] private GameObject[] elementsToHide;

    public event System.Action<bool> OnGameCompleted;

    private bool _gameCompleted = false;
    [HideInInspector] public bool gameCompleted
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

	[SerializeField]
	public TutorialManager tutorial;

    private SessionTimer timer;
    [SerializeField] private int intentos;
    private int fallos;

    private bool alreadyEnded = false;
    private void Awake()
    {
        foreach(GameObject obj in elementsToHide) //amagar tots els elements dins del array
        {
            obj.SetActive(false);
        }

        StartCoroutine(ShowInstructions()); //Corrutina per mostrar les instruccions del minijoc
    }

    private void Start()
    {
        timer = new SessionTimer();
        timer.Start();
        intentos = 1;
        fallos = 0;
    }

    //Rutina perque fagin spawn les notes
    IEnumerator SpawnNotesRoutine()
    {
        while (spawnedNotes < maxNotes)
        {
            if (gameActive) //si el joc esta actiu
            {
                SpawnNextNote();
                spawnedNotes++;
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        yield return new WaitForSeconds(1f);

        EndMinigame();
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

	public void StartMiniGame3()
	{
		Time.timeScale = 1f;
		gameActive = true;

		//Despres de mostrar les instruccions, comença la corrutina de spawn de les notes
		spawnInterval = 90f / bpm;
		StartCoroutine(SpawnNotesRoutine());
	}

	//Mostrar seguent nota
	void SpawnNextNote()
    {
        if(activateNotes.Count >= 2)
        {
            RythmNote oldest = activateNotes[0];
            activateNotes.RemoveAt(0);

            bool wasCurrent = (oldest == currentNote); //tornem a reasignar la nota que s'haura de clicar

            Destroy(oldest.gameObject); //es destrueix la nota mes antiga (la que ha estat mes temps en pantalla)

            if(wasCurrent)
            {
                currentNote = null;
                TryActivateNext();
            }
        }

        Vector3 randomPos = new Vector3(
            Random.Range(transform.position.x + minSpawn.x, transform.position.x + maxSpawn.x),
            Random.Range(transform.position.y + minSpawn.y, transform.position.y + maxSpawn.y),
            Random.Range(transform.position.z + minSpawn.z, transform.position.z + maxSpawn.z)
        );

        GameObject obj = Instantiate(notePrefab, randomPos, Quaternion.identity);

        RythmNote note = obj.GetComponent<RythmNote>();
        note.Init(GetRandomKey(), this);

        activateNotes.Add(note);

        noteQueue.Enqueue(note); //posa a la queue la nota (especie de llista)

        TryActivateNext();
    }

    //Activar la seguent nota (sense marcar)
    void TryActivateNext()
    {
        if (currentNote != null) return;
        if (noteQueue.Count == 0) return;

        currentNote = noteQueue.Dequeue();
        currentNote.SetActiveNote(true);
    }

    public void NoteCompleted(RythmNote note)
    {
        if (note != currentNote) return;

        activateNotes.Remove(note); //treure la nota de la llista si es completa
        Destroy(note.gameObject);
        currentNote = null;

        completedNotes++;

        if (completedNotes >= maxNotes)
        {
            StopAllCoroutines();
            EndMinigame(); //Al finalitzar el minijoc
            return;
        }
            TryActivateNext();
    }

    //Metode per la condicio de finalitzacio del minijoc
    private void EndMinigame()
    {
        gameActive = false;

        fallos += maxNotes - completedNotes;

        if(completedNotes < maxNotes)
        {
            rebootButton.SetActive(true);
            //DialogManager.instance.Show("dialog_23_isbad_1");
            DialogManager.instance.ShowSequence(new string[] {"dialog_23_isbad_1", "dialog_24_isbad_2" });
        }
        else
        {
            if (alreadyEnded) return;
            alreadyEnded = true;

            DialogManager.instance.Show("dialog_22_isgood");
            TerminarMinijuego();
            StartCoroutine(FinishGame());
        }
    }

    IEnumerator FinishGame()
    {
        yield return new WaitForSecondsRealtime(3f);
        gameCompleted = true;
    }

    //Treure teclas random per mostrar
    KeyCode GetRandomKey()
    {
        KeyCode[] keys = { KeyCode.A, KeyCode.S, KeyCode.D };
        return keys[Random.Range(0, keys.Length)];
    }

    public void RestartMinigame()
    {
        Debug.Log("Reiniciando minijuego");

        rebootButton.SetActive(false);

        intentos++;

        StopAllCoroutines();
        gameActive = false;

        spawnedNotes = 0;
        completedNotes = 0;
        currentNote = null;
        noteQueue.Clear();

        foreach(RythmNote n in activateNotes)
        {
            if(n != null)
            {
                Destroy(n.gameObject);
            }
            
        }
        activateNotes.Clear();

        foreach(GameObject obj in elementsToHide)
        {
            obj.SetActive(false);
        }

        StartCoroutine(ShowInstructions());
    }

    private void TerminarMinijuego()
    {
        int tiempo = timer.Stop();

        GameParametersMDB.Instance.SaveMinigameData("MinijuegoCorazon3", tiempo, intentos, null, fallos);
    }
}