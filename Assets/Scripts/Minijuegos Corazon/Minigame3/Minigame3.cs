using System.Collections;
using System.Collections.Generic;
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
    
    public Vector2 minSpawn; // mínim X i Y
    public Vector2 maxSpawn; // màxim X i Y

    private void Update()
    {
        timer += Time.deltaTime;

        //Crear la seguent nota quan toca
        if(currentNoteIndex < sequenceNotes.Count)
        {
            if(timer >= sequenceNotes[currentNoteIndex].time && activateNote == null)
            {
                SpawnNote(sequenceNotes[currentNoteIndex]);
            }
        }

        if(activateNote!= null)
        {
            if (Input.GetKeyDown(activateNote.expectedKey))
            {
                float diff = Mathf.Abs(timer - activateNote.spawnTime);

                if(diff <= hitWindow)
                {
                    Debug.Log("HIT!!");
                    Destroy(activateNote.gameObject);
                    activateNote = null;
                    currentNoteIndex++;

                    if(currentNoteIndex >= sequenceNotes.Count)
                    {
                        CompleteMinigame();
                    }
                }
                else
                {
                    Debug.Log("FAIL (mal timing)");
                }
            }
        }


    }

    void SpawnNote(NoteData data)
    {
        float x = Random.Range(minSpawn.x, maxSpawn.x);
        float y = Random.Range(minSpawn.y, maxSpawn.y);
        Vector3 randomPos = new Vector3(x, y, 0);

        GameObject obj = Instantiate(notePrefab, randomPos, Quaternion.identity);

        activateNote = obj.GetComponent<RythmNote>();
        activateNote.Init(data.key, timer);
    }



    void CompleteMinigame()
    {
        Debug.Log("Minijioc ritrmic completat");
        if(heart != null)
        {
            heart.PlayHeartAnimation(); //Animacio amb dotween
        }
        
        
    }


}
