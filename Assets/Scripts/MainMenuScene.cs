using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScene : MonoBehaviour
{

    public void setCanvasActive(GameObject canva) 
    {
        canva.SetActive(true);
    
    }

    public void setCanvasNotActive(GameObject canva)
    {
        canva.SetActive(false);
    }

    public void changeToFreeScene()
    {
        SceneManager.LoadScene("FreeGame");
    }

    public void changeToLearnScene()
    {
        SceneManager.LoadScene("LearnGame");
    }

    public void quitGame()
    {
        Application.Quit();
    }



}
