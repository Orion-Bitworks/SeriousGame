using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Minigame1 : MonoBehaviour
{
    public DragAndDrop[] draggableValves; //array de objetos que son arrastables
    int totalValves; //valvulas totales
    int placedValves; //valvulas colocadas

    public int correct = 0; //Aciertos

    public TextMeshProUGUI remainObjectsToDrag; //Texto de objetos que hay que arrastrar

    public FasesMinigames phasesManager; //Instancia del script fasesMinigames

    [SerializeField] Button CheckButton;

    [SerializeField]private PopUp popUpManager;

    private void Awake()
    {
        CheckButton.onClick.AddListener(checkPlacementButton); //Listener
    }
    void Start()
    {
        totalValves = draggableValves.Length; //el total de objetos son la cantidad de objetos que haya en el array
        showInfo();
    }

    public void objectsRemaining()
    {
        placedValves = 0;

        foreach (DragAndDrop obj in draggableValves) //para cada objeto dragAndDrop que este dentro del array
        {
            if (obj.placed)
            {
                placedValves++; //suma 1 si el objeto esta puesto

            }

        }

        showInfo();
    }


    //Metodo para mostrar la informacion de las valvulas
    void showInfo() 
    {
        remainObjectsToDrag.text = placedValves + " / " + totalValves; 
    }

    public void checkPlacementButton()
    {

        correct = 0;

        foreach (DragAndDrop obj in draggableValves)
        {
            // Tiene que estar en una DropArea
            if (obj.CurrentDropArea != null)
            {
                DropArea drop = obj.CurrentDropArea.GetComponent<DropArea>();

                // La dropArea existe
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
                            correct++; //suma 1 a los aciertos
                        }
                    }
                }
            }
        }

        Debug.Log("Objetos correctamente colocados: " + correct + " / " + draggableValves.Length);

        if (correct == draggableValves.Length) // Si el numero de aciertos es igual al numero de valvulas que hay en el array
        {
            foreach (DragAndDrop obj in draggableValves)
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

            phasesManager.PasarAFase2(); //pasa a la siguiente fase
            popUpManager.ShowPopUp("Has acabat el primer minijoc!", 2f);

        }

    }
}
