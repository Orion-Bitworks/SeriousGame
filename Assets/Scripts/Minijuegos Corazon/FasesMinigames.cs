using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FasesMinigames : MonoBehaviour
{
    private bool stageEnded;
    public string nextStage;

    public void WinLevel()
    {
        Debug.Log("You win the stage!");

        if(nextStage != "")
        {

        }

        stageEnded = true;
    }

}
