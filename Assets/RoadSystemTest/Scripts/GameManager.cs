using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }    // Lo hacemos Singleton

    public bool isPlaying = false;                              // Booleano para saber si el sistema de bolitas está en marcha o no

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Pone el sistema de bolitas en marcha
    /// </summary>
    public void Play()
    {
        isPlaying = true;
    }

    /// <summary>
    /// Pausa el sistema de bolitas
    /// </summary>
    public void Pause()
    {
        isPlaying = false;
    }
}

