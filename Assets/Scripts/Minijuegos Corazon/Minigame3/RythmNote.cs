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



}


