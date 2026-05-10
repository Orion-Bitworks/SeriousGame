using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class LanguageSwitcher : MonoBehaviour
{
	private const string LanguageKey = "SelectedLanguage";

	[SerializeField] private Toggle[] toggles; 

	private void Start()
	{
		int savedIndex = PlayerPrefs.GetInt(LanguageKey, 0);

		// Desactivar listeners
		foreach (var t in toggles)
			t.onValueChanged.RemoveAllListeners();

		// Aplicar idioma guardado
		LocalizationSettings.SelectedLocale =
			LocalizationSettings.AvailableLocales.Locales[savedIndex];

		// Marcar el toggle correcto sin disparar eventos
		toggles[savedIndex].SetIsOnWithoutNotify(true);

		// Volver a activar listeners
		for (int i = 0; i < toggles.Length; i++)
		{
			int index = i; // evitar captura incorrecta
			toggles[i].onValueChanged.AddListener((isOn) =>
			{
				if (isOn)
					SetLanguage(index);
			});
		}
	}

	private void SetLanguage(int index)
	{
		AudioController.Instance.PlaySFX(SFX.Menu, (int)MenuSFX.Tick);

		LocalizationSettings.SelectedLocale =
			LocalizationSettings.AvailableLocales.Locales[index];

		PlayerPrefs.SetInt(LanguageKey, index);
		PlayerPrefs.Save();
	}
}
