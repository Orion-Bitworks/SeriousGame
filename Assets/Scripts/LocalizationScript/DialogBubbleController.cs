using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class DialogBubbleController : MonoBehaviour
{

	public LocalizeStringEvent dialogText;
	private bool continuePressed = false;

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
