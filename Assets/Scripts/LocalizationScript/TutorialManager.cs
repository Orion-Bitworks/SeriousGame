using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
	[SerializeField] GameObject[] carpetas;

	private GameObject activeTutorial;

	public event Action OnTutorialClosed;
    private bool tutorialClosed = false;

    public void ShowTutorial(int id)
	{

		if (DialogManager.IsDialogActive)
		{
			DialogManager.pendingEvents.Enqueue(() => ShowTutorial(id));
			return;
		}

		StartCoroutine(ShowAndWait(id));
	}

	IEnumerator ShowAndWait(int id)
	{
        if (activeTutorial != null)
        {
            activeTutorial.gameObject.SetActive(false);
        }

        activeTutorial = carpetas[id];
        activeTutorial.gameObject.SetActive(true);

        DialogManager.instance.grid.SetActive(false);
        DialogManager.IsDialogActive = true;

		tutorialClosed = false;

        yield return new WaitUntil(() => tutorialClosed);

        DialogManager.IsDialogActive = false;

        DialogManager.instance.grid.SetActive(true);

    }

	public void HideActiveTutotrial()
	{
		if (activeTutorial != null)
		{
			activeTutorial.gameObject.SetActive(false);
		}

		tutorialClosed = true;

		// Avisar a quien esté esperando
		OnTutorialClosed?.Invoke();
	}
}
