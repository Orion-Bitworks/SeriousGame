using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class Minigame3 : MonoBehaviour
{
    public List<NoteData> sequenceNotes; //Sequencia de notes
    public GameObject notePrefab; //Prefan del cercle
    public Transform spawnPoint; //On apareixeran les notes
    public float hitWindow; //Marge de temps

    private int currentNoteIndex = 0;
    private float timer = 0f;
    private RythmNote activateNote;

    public FasesMinigames phasesManager;
    public HeartController heart;

    [Header("Zona de spawn aleatori")] 
    
    public Vector3 minSpawn; // mínim X i Y
    public Vector3 maxSpawn; // màxim X i Y


    public float bpm = 110f;
    public float spawnInterval;


    private void Start()
    {
        spawnInterval = 60 / bpm;
        StartCoroutine(SpawnNotesRoutine());
        //SpawnNextNote();
    }

    private void Update()
    {
        if(activateNote != null)
        {
            if (Input.GetKeyDown(activateNote.expectedKey))
            {
                Debug.Log("Correcto!");
                Destroy(activateNote.gameObject);
                activateNote = null;

                currentNoteIndex++;

                if(currentNoteIndex >= sequenceNotes.Count)
                {
                    CompleteMinigame();
                }
                else
                {
                    SpawnNextNote();
                }
            }
        }        
    }

    IEnumerator SpawnNotesRoutine()
    {
        while (true)
        {
            SpawnNextNote();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    void SpawnNextNote()
    {
        NoteData data = sequenceNotes[currentNoteIndex];

        float x = Random.Range(minSpawn.x, maxSpawn.x);
        float y = Random.Range(minSpawn.y, maxSpawn.y);
        float z = Random.Range(minSpawn.z, maxSpawn.z);

        Vector3 randomPos = new Vector3(x, y, z);

        GameObject obj = Instantiate(notePrefab, randomPos, Quaternion.identity);

        activateNote = obj.GetComponent<RythmNote>();
        activateNote.Init(data.key);
    }

    void CompleteMinigame()
    {
        Debug.Log("Minijuego completado!");

        if(heart != null)
        {
            heart.PlayHeartAnimation();
        }
    }



}
