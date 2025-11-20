using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScene : MonoBehaviour
{
    [SerializeField] GameObject[] canvas;
    [SerializeField] Scene[] scenes;

   

    bool pressed = false;
    // Start is called before the first frame update
    void Start()
    {
      

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    


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
