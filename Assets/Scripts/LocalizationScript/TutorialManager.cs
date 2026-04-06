using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialManager : MonoBehaviour
{
	[SerializeField]
	private TutorialController[] controller;

	private TutorialController activeTutorial;

	[SerializeField] GameObject carpeta;

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

		activeTutorial = controller[id];
		carpeta.SetActive(true);
		activeTutorial.gameObject.SetActive(true);

		activeTutorial.ResetTutorial();

	}

	public void MoveCarpeta(int move)
	{

			carpeta.transform.position += Camera.main.transform.forward * move;
		
	}

	public void HideActiveTutotrial()
	{
		if (activeTutorial != null)
		{
			activeTutorial.gameObject.SetActive(false);
		}
	}
}
