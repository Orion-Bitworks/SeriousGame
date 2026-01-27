using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour
{
    public DragAndDrop[] draggagleObjects; //array de objetos que son arrastables
    int totalObjects;
    int placedObjects;

    public TextMeshProUGUI objetsToDragInfoDone;

    [SerializeField] Button CheckButton;

    private void Awake()
    {
        CheckButton.onClick.AddListener(checkPlacementButton);
    }
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

        foreach (DragAndDrop obj in draggagleObjects)
        {
            // 1. Tiene que estar en una DropArea
            if (obj.CurrentDropArea != null)
            {
                DropArea drop = obj.CurrentDropArea.GetComponent<DropArea>();

                // La dropArea exista
                if (drop != null)
                {
                    // Comprueba que sea la pieza correcta
                    if (drop.valveType == obj.valveType)
                    {
                        // Comrobar rotación correcta usando quaternions
                        Quaternion currentRot = obj.transform.rotation;
                        float angleDiff = Quaternion.Angle(currentRot, drop.requiredRotation);

                        if (angleDiff <= drop.rotationTolerance) //la rotationTolerance esta explicada en dropArea
                        {
                            correct++;
                        }
                    }
                }
            }
        }

        Debug.Log("Objetos correctamente colocados: " + correct + " / " + draggagleObjects.Length);
    }


}
