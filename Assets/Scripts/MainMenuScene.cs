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

    public void StartCarreterasMiniGame()
    {
        SceneManager.LoadScene("RoadSystemTest");
    }

    public void Start3DCorazonMiniGame()
    {
        SceneManager.LoadScene("PuzzleScene");
    }

	public void StartActivarCorazonMiniGame()
	{
		SceneManager.LoadScene("TestMinijuegosCorazon");
	}

    public void StartIntercambioDeGasesMiniGame()
    {
        SceneManager.LoadScene("AlvMiniGame");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuGame");
    }

    public void ReturnToHeartMinigames()
    {
        SceneManager.LoadScene("TestMinijuegosCorazon");
    }

	public void quitGame()
    {
        Application.Quit();
    }



}
