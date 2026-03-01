using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPauseController : MonoBehaviour
{
	[SerializeField] GameObject menuPausePanel;
	[SerializeField] GameObject[] allOptionPanels;
	GameObject currentPanel;


	private void Start()
	{
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
			if (!menuPausePanel.activeSelf)
			{
				OpenMenuPause();
			}
			else
			{
				CloseMenuPause();
			}
		}
	}


	private void OpenMenuPause()
	{
		menuPausePanel.SetActive(true);
		Time.timeScale = 0f;
	}


	public void ChangeMenu(GameObject activePanel)
	{
		if (currentPanel == activePanel) return;

		currentPanel.SetActive(false);
		currentPanel = activePanel;
		currentPanel.SetActive(true);

	}


	public void CloseMenuPause()
	{
		menuPausePanel.SetActive(false);
		Time.timeScale = 1f;
	}


}
