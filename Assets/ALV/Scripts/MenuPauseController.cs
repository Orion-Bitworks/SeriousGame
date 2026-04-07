using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPauseController : MonoBehaviour
{
	[SerializeField] GameObject pause;
	[SerializeField] GameObject menuPausePanel;
	[SerializeField] GameObject[] allOptionPanels;
	GameObject currentPanel;
	GameObject miniHeart;

	private void Start()
	{
		pause.SetActive(false);
		menuPausePanel.SetActive(false);
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
			if (!pause.activeSelf)
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
		if (SceneManager.GetActiveScene().name == "RoadSystemTest")
		{
			GameManager.Instance.isPlaying = true;
			miniHeart = FindAnyObjectByType<OrganDrag3D>().gameObject;
			miniHeart.SetActive(false);
		}
	}

	public void ClosePause()
	{
		pause.SetActive(false);
		Time.timeScale = 1f;

        if (SceneManager.GetActiveScene().name == "RoadSystemTest")
		{
            GameManager.Instance.isPlaying = false;
            miniHeart.SetActive(true);
        }
	}

	public void OpenOptionMenuPause()
	{
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

    }

	public void ReturnToMenu()
	{
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuGame");
	}
}
