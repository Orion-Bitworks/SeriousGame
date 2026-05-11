using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class DialogBubbleController : MonoBehaviour
{

	public LocalizeStringEvent dialogText;
	
	private bool continuePressed = false;


	private void Update()
	{
		// No avanzar si el juego está pausado
		if (Time.timeScale == 0f)
			return;

		// No avanzar si el diálogo NO está visible
		if (!gameObject.activeInHierarchy)
			return;

		// Avanza con cualquier tecla excepto ESC
		if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape))
		{
			AudioController.Instance.PlaySFX(SFX.Menu, (int)MenuSFX.Click);
			continuePressed = true;
		}
	}



	public void SetKey(string key)
	{
		dialogText.StringReference.TableEntryReference = key;
		dialogText.RefreshString();
	}


	public void OnContinuePressed()
	{
		continuePressed = true;
	}

	public bool WasContinuePressed()
	{
		if (continuePressed)
		{
			continuePressed = false;
			return true;
		}
		return false;
	}
}
