using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageSwitcher : MonoBehaviour
{
	public void SetLanguage(int index)
	{
		LocalizationSettings.SelectedLocale =
			LocalizationSettings.AvailableLocales.Locales[index];
	}
}
