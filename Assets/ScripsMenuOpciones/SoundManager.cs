using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;

    private const string MusicKey = "MusicVolume";
    private const string SFXKey = "SFXVolume";
    // Start is called before the first frame update
    void Start()
    {
        float musicValue = PlayerPrefs.GetFloat(MusicKey, 1f);
        float sfxValue = PlayerPrefs.GetFloat(SFXKey, 1f);

        musicSlider.value = musicValue;
        sfxSlider.value = sfxValue;

        SetMusicVolume(musicValue);
        SetSFXVolume(sfxValue);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);


    }
    /// <summary>
    /// Este metodo nos guardara los valores del volumen de efectos de sonido
    /// Hay que añadirlo al slider de sfx del menu de opciones, en el:
    /// - Canvas / PanelOptions/ SoundPanel / SFXSlider
    /// 
    /// </summary>
    /// <param name="sfxValue"></param>
    private void SetSFXVolume(float sfxValue)
    {
        mixer.SetFloat(MusicKey, Mathf.Log10(Mathf.Max(sfxValue, 0.0001f)) * 20f);


        PlayerPrefs.SetFloat(SFXKey, sfxValue);

    }
    /// <summary>
    /// Este metodo nos guarda los valores del volumen de la musica del juego
    /// Hay que añadirlo al slider de sfx del menu de opciones, en el:
    /// - Canvas / PanelOptions/ SoundPanel / SFXSlider
    /// </summary>
    /// <param name="musicValue"></param>
    private void SetMusicVolume(float musicValue)
    {
        mixer.SetFloat(MusicKey, Mathf.Log10(Math.Max(musicValue, 0.0001f)) * 20);
        PlayerPrefs.SetFloat(MusicKey, musicValue);
    }
}
