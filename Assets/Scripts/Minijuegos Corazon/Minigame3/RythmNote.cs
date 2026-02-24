using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RythmNote : MonoBehaviour
{
    public KeyCode expectedKey;
    //public float spawnTime;

    public TextMeshPro textKey;


    public void Init(KeyCode key)
    {
        expectedKey = key;

        if(textKey != null)
        {
            textKey.text = key.ToString();
        }
    }











    /*public void Init(KeyCode key, float spawnTimeValue)
    {
        expectedKey = key;
        spawnTime = spawnTimeValue;

        if (textKey != null)
            textKey.text = expectedKey.ToString();
    }

    private void Update()
    {
        transform.Translate(Vector3.down * 3f * Time.deltaTime);
    }*/
}


