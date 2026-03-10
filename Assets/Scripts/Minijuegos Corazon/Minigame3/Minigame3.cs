using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

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

    private List<RythmNote> activateNotes = new List<RythmNote>(); //lista per controlar les notes actives

    [SerializeField] public GameObject rebootButton; //boto per reiniciar el minijoc en cas de haver-ho fet malament

    //ELEMENTOS A ESCONDER
    [SerializeField] private GameObject[] elementsToHide;


    private void Awake()
    {
        foreach(GameObject obj in elementsToHide) //amagar tots els elements dins del array
        {
            obj.SetActive(false);
        }

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
            Random.Range(minSpawn.x, maxSpawn.x),
            Random.Range(minSpawn.y, maxSpawn.y),
            0
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
            EndMinigame(); //Al finalitzar el minijoc
            return;
        }
            TryActivateNext();
    }

    //Metode per la condicio de finalitzacio del minijoc
    private void EndMinigame()
    {
        gameActive = false;

        if(completedNotes >= maxNotes)
        {
            popUpManager.ShowPopUp("Has acabat el tercer minijoc, Felicitats! Has acabat tots els minijocs correctament", 3f);
        }
        else
        {
            rebootButton.SetActive(true);
            popUpManager.ShowPopUp($"Has fet un total de {completedNotes} de {maxNotes}, torna a intentar-ho de nou", 4);
        }
    }


    //Treure teclas random per mostrar
    KeyCode GetRandomKey()
    {
        KeyCode[] keys = { KeyCode.A, KeyCode.S, KeyCode.D };
        return keys[Random.Range(0, keys.Length)];
    }

    public void RestartMinigame()
    {
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
}