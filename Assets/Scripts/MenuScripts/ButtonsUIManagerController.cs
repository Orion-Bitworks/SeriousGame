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
	Image currentSecondaryButton;

	[SerializeField] Button buttonActive;
	[SerializeField] Button keyboardButton;

	private void Start()
	{
		if (buttonActive != null)
		{
			currentActiveButton = buttonActive.GetComponent<Image>();

			Color c = currentActiveButton.color;
			c.a = alphaActive;
			currentActiveButton.color = c;

		}

		if (keyboardButton != null)
		{
			currentSecondaryButton = keyboardButton.GetComponent<Image>();

			Color c = currentSecondaryButton.color;
			c.a = alphaActive;
            currentSecondaryButton.color = c;
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

	public void SelectSecondaryButton(Button button)
	{
		Image img = button.GetComponent<Image>();

		if (currentSecondaryButton != null)
		{
			Color c = currentSecondaryButton.color;
			c.a = alphaInactive;
			currentSecondaryButton.color = c;
		}

		Color newC = img.color;
		newC.a = alphaActive;
		img.color = newC;

		currentSecondaryButton = img;
	}
}
