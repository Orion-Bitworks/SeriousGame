using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class TutorialManager : MonoBehaviour
{
	[SerializeField]
	private TutorialController[] controller;

	private TutorialController activeTutorial;

	[SerializeField] GameObject carpeta;
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

		activeTutorial = controller[id];
		carpeta.SetActive(true);
		activeTutorial.gameObject.SetActive(true);

		activeTutorial.ResetTutorial();



	}

	public void MoveCarpeta3D(int move)
	{

		carpeta.transform.position += Camera.main.transform.forward * move;
		
	}

	public void MoveCarpetaMiniHeart() 
	{
		carpeta.transform.localPosition = new Vector3(0f, -0.3f, 1.14f);
		carpeta.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
	}

	public void ReposicionarCarpeta()
	{
		carpeta.transform.localPosition = new Vector3(0f, -0.3f, 2f);
		carpeta.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
	}

	public void HideActiveTutotrial()
	{
		if (activeTutorial != null)
		{
			activeTutorial.gameObject.SetActive(false);
		}

		carpeta.SetActive(false);

		// Avisar a quien esté esperando
		OnTutorialClosed?.Invoke();
	}
}
