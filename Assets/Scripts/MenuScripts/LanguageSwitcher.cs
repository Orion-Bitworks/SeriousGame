using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageSwitcher : MonoBehaviour
{
	public void SetLanguage(int index)
	{
        AudioController.Instance.PlaySFX(SFX.Menu, (int)MenuSFX.Tick);
        LocalizationSettings.SelectedLocale =
			LocalizationSettings.AvailableLocales.Locales[index];
	}
}
