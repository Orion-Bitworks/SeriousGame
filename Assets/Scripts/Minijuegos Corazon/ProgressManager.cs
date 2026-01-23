using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public DragAndDrop[] draggagleObjects; //array de objetos que son arrastables
    int totalObjects;
    int placedObjects;

    public TextMeshProUGUI objetsToDragInfoDone;


    void Start()
    {
        totalObjects = draggagleObjects.Length; //el total de objetos son la cantidad de objetos que haya en el array
        showInfo();
    }


    public void objectsRemaining()
    {
        placedObjects = 0;

        foreach(DragAndDrop obj in draggagleObjects) //para cada objeto dragAndDrop que este dentro del array
        {
            if (obj.placed)
            {
                placedObjects++; //suma 1 si el objeto esta puesto

            }
            
        }
        showInfo();

    }

    void showInfo()
    {
        objetsToDragInfoDone.text = placedObjects + " / " + totalObjects;
    }

    public void checkPlacementButton()
    {
        int correct = 0;
        foreach(DragAndDrop obj in draggagleObjects)
        {
            if(obj.CurrentDropArea != null)
            {
                DropArea drop = obj.CurrentDropArea.GetComponent<DropArea>();
                if (drop != null && drop.valveType == obj.valveType)
                {
                    correct++;
                }
            }
        }
        Debug.Log("Objetos correctamente colocados: " + correct + " / " + draggagleObjects.Length);
    }

}
