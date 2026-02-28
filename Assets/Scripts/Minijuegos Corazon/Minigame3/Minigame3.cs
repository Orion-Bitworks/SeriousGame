using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] PopUp popUpManager;
    private bool gameActive = false; //Variable per pausar el joc

    private void Awake()
    {
        StartCoroutine(ShowInstructions()); //Corrutina per mostrar les instruccions del minijoc
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
    }

    IEnumerator ShowInstructions()
    {
        gameActive = false;
        Time.timeScale = 0f;

        popUpManager.ShowPopUp(
            "Et sortira un joc on apareixeran una serie de boles. Has de pitjar la tecla que contigui la bola marcada en verd. Tens un cert temps per clicar, ja que van el ritme de la macarena",
            4f
        );

        // Espera en temps real 
        yield return new WaitForSecondsRealtime(6f);

        Time.timeScale = 1f;
        gameActive = true;

        //Despres de mostrar les instruccions, comença la corrutina de spawn de les notes
        spawnInterval = 60f / bpm;
        StartCoroutine(SpawnNotesRoutine());
    }

    //Mostrar seguent nota
    void SpawnNextNote()
    {
        Vector3 randomPos = new Vector3(
            Random.Range(minSpawn.x, maxSpawn.x),
            Random.Range(minSpawn.y, maxSpawn.y),
            0
        );

        GameObject obj = Instantiate(notePrefab, randomPos, Quaternion.identity);

        RythmNote note = obj.GetComponent<RythmNote>();
        note.Init(GetRandomKey(), this);

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

        Destroy(note.gameObject);
        currentNote = null;

        completedNotes++;

        if (completedNotes >= maxNotes)
        {
            gameActive = false; // Bloqueja el joc
            Debug.Log("HAS ACABAT!  Has completat totes les notes correctament!");
            popUpManager.ShowPopUp("Has acabat el tercer minijoc, Felicitats! Has acabat tots els minijocs correctament", 3f);
            return;
        }

        TryActivateNext();
    }

    //Treure teclas random per mostrar
    KeyCode GetRandomKey()
    {
        KeyCode[] keys = { KeyCode.A, KeyCode.S, KeyCode.D };
        return keys[Random.Range(0, keys.Length)];
    }
}