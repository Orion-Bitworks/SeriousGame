using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionMenu : MonoBehaviour
{
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] Toggle fullscreenToogle;

    private Resolution[] allResolution;
    private List<Resolution> resolutionList = new List<Resolution>();
    private const string ResolutionKey = "SelectedResolution";
    private const string FullscreeneKey = "Fullscreen";


    void Start()
    {
        //obtenemos todas las resoluciones
        allResolution = Screen.resolutions;
        //evitamos duplicados por tamaño
        HashSet<string> seen = new HashSet<string>();
        List<string> options = new List<string>();

        foreach (Resolution res in allResolution)
        {
            string key = res.width + "x" + res.height;
            if (!seen.Contains(key))
            {
                seen.Add(key);
                resolutionList.Add(res);
                options.Add(key);
            }
        }

        //limpiamos el dropdown y guardamos las resoluciones en este
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);

        // cargamos la resolucionn guardada y actualiza el dropdown
        int savedIndex = PlayerPrefs.GetInt(ResolutionKey, resolutionList.Count - 1);
        resolutionDropdown.value = savedIndex;
        resolutionDropdown.RefreshShownValue();

        //cargar pantalla guardada y usamos un true por defecto
        bool isFullScreen = PlayerPrefs.GetInt(FullscreeneKey,1) == 1;
        fullscreenToogle.isOn = isFullScreen;
        ApplyResolution(savedIndex, isFullScreen);

        //añadimos listeners para hacer los cambios cuando el usuario cambie
        //las resoluciones o la pantalla completa
        resolutionDropdown.onValueChanged.AddListener(onResolutionChanged);
        fullscreenToogle.onValueChanged.AddListener(OnFullscreenToggled);

    }

    private void OnFullscreenToggled(bool isFullscreen)
    {
        ApplyResolution(resolutionDropdown.value, isFullscreen);
        PlayerPrefs.SetInt(FullscreeneKey, isFullscreen ? 1 : 0);
    }

    private void onResolutionChanged(int resChanged)
    {
        ApplyResolution(resChanged, fullscreenToogle.isOn);
        PlayerPrefs.SetInt(ResolutionKey, resChanged);
    }

    private void ApplyResolution(int savedIndex, bool isFullScreen)
    {
        Resolution res = allResolution[savedIndex];
        Screen.SetResolution(res.width, res.height, isFullScreen);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
