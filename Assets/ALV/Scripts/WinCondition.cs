using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
	[SerializeField] MolCountSO molCountSO;
	[SerializeField] UICounter ui;
	[SerializeField] GameObject FinishGamePanel;
	[SerializeField] private int pointsToWin;

	private void Start()
	{
		//OnPressResset();
		SetActiveFinishPanel(false);
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
		FinishGamePanel.SetActive(trueFalse);
	}

	public void OnPressResset()
	{
		SceneManager.LoadScene("AlvMiniGame");
		molCountSO.RestoreParameters();
		/*SetActiveFinishPanel(false);*/
		Time.timeScale = 1f;
	}

}
