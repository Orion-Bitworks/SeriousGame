using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
	[SerializeField] TextMeshProUGUI pointsToWinText;

	[SerializeField] private float maxTime = 30f;
	private float currentTime;
	[SerializeField] TextMeshProUGUI timerText;

	private void Start()
	{

		SetActiveFinishPanel(false);
		startPanel.SetActive(true);
		molCountSO.RestoreParameters();
		currentTime = maxTime;
		Time.timeScale = 1f;
		pointsToWinText.text = pointsToWin.ToString();
		pointsToWinText.text = $"/  {pointsToWin}";


	}


	private void Update()
	{
		if (startPanel.activeSelf) return;

		currentTime -= Time.deltaTime;

		int minutes = Mathf.FloorToInt(currentTime / 60);
		int seconds = Mathf.FloorToInt(currentTime % 60);
		timerText.text = $"{minutes:00}:{seconds:00}";


/*
		if (timerText != null)
		{
			timerText.text = Mathf.CeilToInt(currentTime).ToString();

		}*/
		if(currentTime <= 0)
		{
			LoseGame();
		}
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
		//currentTime = maxTime;
	}

	public void DoYouWin()
	{

		if(molCountSO.totalO2 == pointsToWin)
		{
			SetActiveFinishPanel(true);
			Time.timeScale = 0f;
		}
	}


	public void LoseGame()
	{
		SetActiveFinishPanel(true);
		Time.timeScale = 0f;

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
