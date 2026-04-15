using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioUIController : MonoBehaviour
{
    // Apunta a los sliders del menú de pausa
    [Header("Sliders")]
    [SerializeField] Slider generalSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    // Variables para las VCAs del FMOD configuradas
    private FMOD.Studio.VCA generalVCA;
    private FMOD.Studio.VCA musicVCA;
    private FMOD.Studio.VCA sfxVCA;

    private void Start()
    {
        // Obtenemos las VCAs correspondientes
        generalVCA = FMODUnity.RuntimeManager.GetVCA("vca:/General");
        musicVCA = FMODUnity.RuntimeManager.GetVCA("vca:/Music");
        sfxVCA = FMODUnity.RuntimeManager.GetVCA("vca:/SFX");

        // Ponemos los sliders en la posición que tiene en el FMOD
        if (generalVCA.getVolume(out float generalVolume) == FMOD.RESULT.OK) generalSlider.value = generalVolume;
        if (musicVCA.getVolume(out float musicVolume) == FMOD.RESULT.OK) musicSlider.value = musicVolume;
        if (sfxVCA.getVolume(out float sfxVolume) == FMOD.RESULT.OK) sfxSlider.value = sfxVolume;

        // Definimos los sliders para que ejecuten las funciones cuando cambian de valor
        generalSlider.onValueChanged.AddListener(OnGeneralAudioChanged);
        musicSlider.onValueChanged.AddListener(OnMusicAudioChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXAudioChanged);
    }

    // Se llama desde un slider, cambia el volumen de la VCA general según el valor introducido en el slider.
    public void OnGeneralAudioChanged(float value)
    {
        generalVCA.setVolume(value);
    }

    // Se llama desde un slider, cambia el volumen de la VCA de música según el valor introducido en el slider.
    public void OnMusicAudioChanged(float value)
    {
        musicVCA.setVolume(value);
    }

    // Se llama desde un slider, cambia el volumen de la VCA de SFX según el valor introducido en el slider.
    public void OnSFXAudioChanged(float value)
    {
        sfxVCA.setVolume(value);
    }
}
