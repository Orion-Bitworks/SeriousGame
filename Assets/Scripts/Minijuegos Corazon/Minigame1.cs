using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Minigame1 : MonoBehaviour
{
    public DragAndDrop[] draggagleValves; //array de objetos que son arrastables
    int totalValves;
    int placedValves;

    public int correct = 0;

    public TextMeshProUGUI remainObjectsToDrag;

    public FasesMinigames phasesManager;

    [SerializeField] Button CheckButton;

    private void Awake()
    {
        CheckButton.onClick.AddListener(checkPlacementButton);
    }
    void Start()
    {
        totalValves = draggagleValves.Length; //el total de objetos son la cantidad de objetos que haya en el array
        showInfo();
    }

    public void objectsRemaining()
    {
        placedValves = 0;

        foreach (DragAndDrop obj in draggagleValves) //para cada objeto dragAndDrop que este dentro del array
        {
            if (obj.placed)
            {
                placedValves++; //suma 1 si el objeto esta puesto

            }

        }

        showInfo();
    }

    void showInfo()
    {
        remainObjectsToDrag.text = placedValves + " / " + totalValves;
    }

    public void checkPlacementButton()
    {

        correct = 0;

        foreach (DragAndDrop obj in draggagleValves)
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

        Debug.Log("Objetos correctamente colocados: " + correct + " / " + draggagleValves.Length);

        if (correct == draggagleValves.Length)
        {
            phasesManager.PasarAFase2();
        }

    }
}
