using Cinemachine;
using Cinemachine.PostFX;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class MainMenuScene : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera pauseCam;

    [SerializeField] CinemachinePostProcessing postProCamera;

    DepthOfField dof;

    [SerializeField] Canvas menuCanvas;

    private void Start()
	{
		TempLevelHolder.introShown = false;

        postProCamera.m_Profile.TryGetSettings(out dof);

        dof.active = true;
    }
	public void setCanvasActive(GameObject canva) 
    {
        canva.SetActive(true);
        menuCanvas.gameObject.SetActive(false);

        pauseCam.Priority = 2;
    }

    public void setCanvasNotActive(GameObject canva)
    {
        menuCanvas.gameObject.SetActive(true);
        canva.SetActive(false);

        pauseCam.Priority = 0;
    }

    public void StartCarreterasMiniGame()
    {
        AudioController.Instance.StopHeartbeat();
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
