using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsUIManagerController : MonoBehaviour
{
	public float alphaActive = 1f;
	public float alphaInactive = 0f;

	Image currentActiveButton;

	[SerializeField] Button buttonActive;

	private void Start()
	{
		if (buttonActive != null)
		{
			currentActiveButton = buttonActive.GetComponent<Image>();

			Color c = currentActiveButton.color;
			c.a = alphaActive;
			currentActiveButton.color = c;

		}
	}

	public void SelectButton(Button button)
	{
		Image img = button.GetComponent<Image>();

		if (currentActiveButton != null)
		{
			Color color = currentActiveButton.color;
			color.a = alphaInactive;
			currentActiveButton.color = color;

		}

		Color newColor = img.color;
		newColor.a = alphaActive;
		img.color = newColor;

		currentActiveButton = img;

	}
}
