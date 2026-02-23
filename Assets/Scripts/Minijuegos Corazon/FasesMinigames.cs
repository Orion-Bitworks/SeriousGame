using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FasesMinigames : MonoBehaviour
{
    public GameObject fase1Root;
    public GameObject fase2Root;
    public GameObject fase3Root;

    private void Start()
    {
        fase1Root.SetActive(true);
    }

    public void LimpiarSeleccion()
    {
        // Netejar selecció del objecte de la fase anterior al objectSelector
        if (ObjectSelector.currentlySelected != null)
        {
            ObjectSelector.currentlySelected.Deselect();
            ObjectSelector.currentlySelected = null;
        }

        // Netejar selecció del objecte de la fase anterior al dragAndDrop
        if (DragAndDrop.currentlySelected != null)
        {
            DragAndDrop.currentlySelected.selectObj?.Deselect();
            DragAndDrop.currentlySelected = null;
        }
    }


    // Llamar cuando el minijuego 1 termine correctamente
    public void PasarAFase2()
    {
        LimpiarSeleccion();

        fase1Root.SetActive(false);   // Oculta todo el minijuego 1
        fase2Root.SetActive(true);    // Muestra todo el minijuego 2
        fase3Root.SetActive(false);   // Oculta todo el minijuego 3
    }

    public void PasarAFase3()
    {
        LimpiarSeleccion();

        fase1Root.SetActive(false); // Oculta todo el minijuego 1
        fase2Root.SetActive(false); // Oculta todo el minijuego 2
        fase3Root.SetActive(true);  // Muestra todo el minijuego 3
    }

}
