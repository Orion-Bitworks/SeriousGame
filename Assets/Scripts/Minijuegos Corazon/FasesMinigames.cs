using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FasesMinigames : MonoBehaviour
{
    public GameObject fase1Root;
    public GameObject fase2Root;

    private void Start()
    {
        fase1Root.SetActive(true);
    }

    // Llamar cuando el minijuego 1 termine correctamente
    public void PasarAFase2()
    {
        fase1Root.SetActive(false);   // Oculta todo el minijuego 1
        fase2Root.SetActive(true);    // Muestra todo el minijuego 2
    }

}
