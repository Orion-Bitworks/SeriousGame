using Cinemachine.PostFX;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class MenuPauseController : MonoBehaviour
{
	[SerializeField] public GameObject pause;
	[SerializeField] GameObject menuPausePanel;
	[SerializeField] GameObject[] allOptionPanels;
	GameObject currentPanel;

	[SerializeField] CinemachinePostProcessing pipeCamera;
	[SerializeField] CinemachinePostProcessing threeDCamera;
	[SerializeField] CinemachinePostProcessing minigamesCamera;

	DepthOfField dofPipe;
	DepthOfField dof3D;
	DepthOfField dofMini;

	[SerializeField] GridManager gridManager;

	private void Start()
	{
		pause.SetActive(false);
		menuPausePanel.SetActive(false);

        pipeCamera.m_Profile.TryGetSettings(out dofPipe);
        threeDCamera.m_Profile.TryGetSettings(out dof3D);
        minigamesCamera.m_Profile.TryGetSettings(out dofMini);

        dofPipe.active = false;
        dof3D.active = false;
        dofMini.active = false;

        foreach (GameObject panel in allOptionPanels)
		{

			if (panel.name == "SoundPanel")
			{
				panel.SetActive(true);
				currentPanel = panel;

			}
			else
			{
				panel.SetActive(false);
			}

		}

	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (!pause.activeSelf && !menuPausePanel.activeSelf)
			{
				StartPause();
			}
			else
			{
				ClosePause();
			}
		}
	}

	private void StartPause()
	{
		pause.SetActive(true);
		Time.timeScale = 0f;

        dofPipe.active = true;
        dof3D.active = true;
        dofMini.active = true;

		if (SceneManager.GetActiveScene().name == "RoadSystemTest"!)
			gridManager.gameObject.SetActive(false);
	}

	public void ClosePause()
	{
		pause.SetActive(false);
        if (menuPausePanel.activeSelf)
		{
			menuPausePanel.SetActive(false);
		}

        Time.timeScale = 1f;

        dofPipe.active = false;
        dof3D.active = false;
        dofMini.active = false;

        if (SceneManager.GetActiveScene().name == "RoadSystemTest")
            gridManager.gameObject.SetActive(true);
	}

	public void OpenOptionMenuPause()
	{
		pause.SetActive(false);
		menuPausePanel.SetActive(true);
	}

	public void ChangeMenu(GameObject activePanel)
	{
		if (currentPanel == activePanel) return;

		currentPanel.SetActive(false);
		currentPanel = activePanel;
		currentPanel.SetActive(true);

	}

	public void CloseOptionMenuPause()
	{
		menuPausePanel.SetActive(false);
        pause.SetActive(true);
    }

    public void GoToCredits()
    {
        Time.timeScale = 1f;
        DialogManager.pendingEvents.Clear();
        DialogManager.IsDialogActive = false;

        SceneManager.LoadScene("CreditsScene");
    }

    public void ReturnToMenu()
	{
		Time.timeScale = 1f;
		DialogManager.pendingEvents.Clear();
		DialogManager.IsDialogActive = false;

		SceneManager.LoadScene("MainMenuGame");
	}
}
