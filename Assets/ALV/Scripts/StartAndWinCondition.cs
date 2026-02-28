using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartAndWinCondition : MonoBehaviour
{
	[SerializeField] MolCountSO molCountSO;
	[SerializeField] UICounter ui;
	[SerializeField] GameObject finishGamePanel;
	[SerializeField] GameObject startPanel;
	[SerializeField] private int pointsToWin;

	private void Start()
	{

		SetActiveFinishPanel(false);
		startPanel.SetActive(true);
		molCountSO.RestoreParameters();


	}
	private void OnEnable()
	{
		molCountSO.OnValueChanged += DoYouWin;
	}

	private void OnDisable()
	{
		molCountSO.OnValueChanged -= DoYouWin;
	}

	public void StartGame()
	{
		startPanel.SetActive(false);
	}

	public void DoYouWin()
	{

		if(molCountSO.totalO2 == pointsToWin)
		{
			SetActiveFinishPanel(true);
			Time.timeScale = 0f;
		}
	}

	public void SetActiveFinishPanel(bool trueFalse)
	{
		finishGamePanel.SetActive(trueFalse);
	}

	public void OnPressResset()
	{
		SceneManager.LoadScene("AlvMiniGame");
		molCountSO.RestoreParameters();
		Time.timeScale = 1f;
	}

	public void OnPressExit()
	{
		SceneManager.LoadScene("MainMenuGame");
		molCountSO.RestoreParameters();
		Time.timeScale = 1f;
	}
}
