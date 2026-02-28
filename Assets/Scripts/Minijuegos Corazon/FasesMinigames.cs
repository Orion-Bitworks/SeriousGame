using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FasesMinigames : MonoBehaviour
{
    public GameObject fase1Root; //Minijoc1
    public GameObject fase2Root; //Minijoc2
    public GameObject fase3Root; //Minijoc3

    public GameObject venas;

    private void Start()
    {
        fase1Root.SetActive(true); //Activar la primera fase des de un inici
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


    //Quan el minijoc1 acabi, pasara a la seguent fase (2)
    public void PasarAFase2()
    {
        LimpiarSeleccion();

        fase1Root.SetActive(false);   // Oculta tot el minijoc 1
        fase2Root.SetActive(true);    // Mostra tot el minijoc 2
        fase3Root.SetActive(false);   // Oculta tot el minijoc 3

        venas.SetActive(true); //Activar les venes quan pasi a la seguent fase
    }

    //Quan el minijoc2 acabi, pasara a la seguent fase (3)
    public void PasarAFase3()
    {
        LimpiarSeleccion();

        fase1Root.SetActive(false); // Oculta tot el minijoc 1
        fase2Root.SetActive(false); // Oculta tot el minijoc 2 
        fase3Root.SetActive(true);  // Mostra tot el minijoc 3
    }

}
