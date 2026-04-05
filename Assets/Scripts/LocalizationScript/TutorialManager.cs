using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	[SerializeField]
	private TutorialController[] controller;

	private TutorialController activeTutorial;

	public void ShowTutorial(int id)
	{
		if (activeTutorial != null)
		{
			activeTutorial.gameObject.SetActive(false);
		}

		activeTutorial = controller[id];
		activeTutorial.gameObject.SetActive(true);

		activeTutorial.ResetTutorial();

	}

	public void HideActiveTutotrial()
	{
		if (activeTutorial != null)
		{
			activeTutorial.gameObject.SetActive(false);
		}
	}
}
