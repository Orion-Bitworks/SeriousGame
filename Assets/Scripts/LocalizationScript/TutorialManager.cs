using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class TutorialManager : MonoBehaviour
{
	[SerializeField]
	private TutorialController[] controller;

	[SerializeField] GameObject[] carpetas;

	private GameObject activeTutorial;

	public event Action OnTutorialClosed;

	public void ShowTutorial(int id)
	{

		if (DialogManager.IsDialogActive)
		{
			DialogManager.pendingEvents.Enqueue(() => ShowTutorial(id));
			return;
		}

		if (activeTutorial != null)
		{
			activeTutorial.gameObject.SetActive(false);
		}

		activeTutorial = carpetas[id];
		activeTutorial.gameObject.SetActive(true);
	}

	public void HideActiveTutotrial()
	{
		if (activeTutorial != null)
		{
			activeTutorial.gameObject.SetActive(false);
		}

		// Avisar a quien esté esperando
		OnTutorialClosed?.Invoke();
	}
}
