using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigame3 : MonoBehaviour
{
    public float bpm = 110f;
    private float spawnInterval;

    public int maxNotes = 20;
    private int spawnedNotes = 0;

    public GameObject notePrefab;
    public Vector3 minSpawn;
    public Vector3 maxSpawn;

    private Queue<RythmNote> noteQueue = new Queue<RythmNote>();
    private RythmNote currentNote;

    private int completedNotes = 0;

    [SerializeField] PopUp popUpManager;


    private void Start()
    {
        spawnInterval = 60f / bpm;
        StartCoroutine(SpawnNotesRoutine());
    }

    IEnumerator SpawnNotesRoutine()
    {
        while (spawnedNotes < maxNotes)
        {
            SpawnNextNote();
            spawnedNotes++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

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

        noteQueue.Enqueue(note);

        TryActivateNext();
    }

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
            Debug.Log("HAS ACABAT! 🎉 Has completat totes les notes correctament!");
            popUpManager.ShowPopUp("Has acabat el tercer minijoc!");
            return;
        }

        TryActivateNext();
    }

    KeyCode GetRandomKey()
    {
        KeyCode[] keys = { KeyCode.A, KeyCode.S, KeyCode.D };
        return keys[Random.Range(0, keys.Length)];
    }
}