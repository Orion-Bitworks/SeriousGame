using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;

public class Minigame2 : MonoBehaviour
{
    public DragAndDrop[] draggagleVeins; //array de objetos que son arrastables
    int totalVeins;
    int placedVeins;

    public int correct = 0;

    public TextMeshProUGUI remainVeinstoDrag;

    [SerializeField] Button CheckButton;

    public FasesMinigames phasesManager; //Instancia del script fasesMinigames

    [SerializeField]
    private PopUp popUpManager;




    private void Awake()
    {
        CheckButton.onClick.AddListener(checkPlacementButton);
    }
    void Start()
    {
        totalVeins = draggagleVeins.Length; //el total de objetos son la cantidad de objetos que haya en el array
        showInfo();
    }

    public void objectsRemaining()
    {
        placedVeins = 0;

        foreach (DragAndDrop obj in draggagleVeins) //para cada objeto dragAndDrop que este dentro del array
        {
            if (obj.placed)
            {
                placedVeins++; //suma 1 si el objeto esta puesto

            }

        }

        showInfo();
    }

    void showInfo()
    {
        remainVeinstoDrag.text = placedVeins + " / " + totalVeins;
    }

    public void checkPlacementButton()
    {

        correct = 0;

        foreach (DragAndDrop obj in draggagleVeins)
        {
            // Tiene que estar en una DropArea
            if (obj.CurrentDropArea != null)
            {
                DropArea drop = obj.CurrentDropArea.GetComponent<DropArea>();

                // La dropArea exista
                if (drop != null)
                {
                    // Comprueba que sea la pieza correcta
                    if (drop.valveType == obj.valveType)
                    {
                        // Comprobar rotación correcta usando quaternions
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

        Debug.Log("Objetos correctamente colocados: " + correct + " / " + draggagleVeins.Length);

        if (correct == draggagleVeins.Length) // Si el numero de aciertos es igual al numero de venas que hay en el array
        {
            foreach (DragAndDrop obj in draggagleVeins)
            {
                if (obj.CurrentDropArea != null)
                {
                    DropArea drop = obj.CurrentDropArea.GetComponent<DropArea>();
                    if (drop != null && drop.valveType == obj.valveType)
                    {
                        Quaternion currentRot = obj.transform.rotation;
                        float angleDiff = Quaternion.Angle(currentRot, drop.requiredRotation);
                        if (angleDiff <= drop.rotationTolerance)
                        {
                            obj.locked = true;
                            obj.GetComponent<Collider>().enabled = false;

                        }
                    }
                }
            }

            phasesManager.PasarAFase3(); //pasa a la siguiente fase
            popUpManager.ShowPopUp("Has acabat el segon minijoc!");

        }

    }
}
