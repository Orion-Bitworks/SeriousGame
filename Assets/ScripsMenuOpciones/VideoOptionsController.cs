using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoOptionsController : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] Toggle fullscreenToggle;

    [Header("Guardado de resoluciones")]
    private Resolution[] allResolutions;
    private List<Resolution> filteredResolutions = new List<Resolution>();

    private const string ResolutionKey = "SelectedRessolution";
    private const string FullscreenKey = "Fullscreen";

    private void Start()
    {
        //obtencion de resoluciones y evitamos los duplocados por tamaño
        allResolutions=Screen.resolutions;
        //creamos HashSet, es una coleccion de elementos unicos
        //que nos permite determinar si un objeto ya esta en un conjunto o no
        HashSet<string> seen = new HashSet<string> ();
        List<string> options = new List<string> ();

		//Recorremos y añadimos todas las resoluciones
		foreach (Resolution resolution in allResolutions)
		{
			float aspect = (float)resolution.width / resolution.height;

			// Solo aceptamos resoluciones 16:9
			if (Mathf.Abs(aspect - (16f / 9f)) > 0.01f)
				continue;

			string key = resolution.width + "x" + resolution.height;

			if (!seen.Contains(key))
			{
				seen.Add(key);
				filteredResolutions.Add(resolution);
				options.Add(key);
			}
		}
		//limpiamos el dropdown y luego guardamos las resoluciones
		resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);

        //cargamos la resolucion guardada uy actualizamos el dropdown
        int savedIndex = PlayerPrefs.GetInt(ResolutionKey, filteredResolutions.Count - 1);
        if (savedIndex < 0 || savedIndex >= filteredResolutions.Count)
        {
            savedIndex = filteredResolutions.Count-1;
        }
        
        
        resolutionDropdown.value = savedIndex;
        resolutionDropdown.RefreshShownValue();
        //con esto nos aseguramos que siempre tengamos la lista actualizada
            
        //Cargamos la pantalla completa guardada y usamos true por defecto
        bool isFullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) == 1;
        fullscreenToggle.isOn = isFullscreen;
        ApplyResoluton(savedIndex, isFullscreen);

        // añadimos listeners para hacer los cambios cuadno el usuario haga el cambio,
        // tanto de resoluciones como de pantalla completa
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
    }
    /// <summary>
    /// Con este metodo aplicamos los cambios en pantalla completa y
    /// lo guardamos en player prefs
    /// </summary>
    /// <param name="isFullscreen"></param>
    private void OnFullscreenChanged(bool isFullscreen)
    {
        AudioController.Instance.PlaySFX(SFX.Menu, (int)MenuSFX.Tick);
        ApplyResoluton(resolutionDropdown.value, isFullscreen);
        PlayerPrefs.SetInt(FullscreenKey, isFullscreen ? 1:0);
    }

    private void OnResolutionChanged(int index)
    {
        AudioController.Instance.PlaySFX(SFX.Menu, (int)MenuSFX.Click);
        ApplyResoluton(index, fullscreenToggle.isOn);
        PlayerPrefs.SetInt(ResolutionKey, index);
    }

    private void ApplyResoluton(int savedIndex, bool isFullscreen)
    {
        Resolution res = filteredResolutions[savedIndex];
        Screen.SetResolution(res.width, res.height, isFullscreen);
    }
}
