using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class TutorialLocalizationController : MonoBehaviour
{

	public List<LocalizeStringEvent> subtitleText;
	public List<LocalizeStringEvent> explanationText;
	public List<LocalizeStringEvent> keysText;

	[SerializeField]
	private string[] subtitleKeys;

	[SerializeField]
	private string[] explanationKeys;

	[SerializeField]
	private string[] keysKeys;

	private int index = 0;

	public void SetPage(int page)
	{
		index = Mathf.Clamp(page, 0, subtitleKeys.Length - 1);

		subtitleText[page].StringReference.TableEntryReference = subtitleKeys[page];
		explanationText[page].StringReference.TableEntryReference = explanationKeys[page];
		keysText[page].StringReference.TableEntryReference = keysKeys[page];

		subtitleText[page].RefreshString();
		explanationText[page].RefreshString();
		keysText[page].RefreshString();
	}

}
