using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Minigame2 : MonoBehaviour
{
    public DragAndDrop[] draggagleVeins; //array de objetos que son arrastables
    int totalVeins; 
    int placedVeins; //venes colocades

    public int correct = 0;

    public TextMeshProUGUI remainVeinstoDrag;

    [SerializeField] Button CheckButton; //Boto per comprovar

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

        foreach (DragAndDrop obj in draggagleVeins) //per a cada objecte dragAndDrop que estigui dins del array
        {
            if (obj.placed)
            {
                placedVeins++; //suma 1 si el objecte esta posat

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
            // Ha de estar en una dropArea
            if (obj.CurrentDropArea != null)
            {
                DropArea drop = obj.CurrentDropArea.GetComponent<DropArea>();

                // Que la dropArea existeixi
                if (drop != null)
                {
                    // Comprova que sigui la peça correcte
                    if (drop.valveType == obj.valveType)
                    {
                        // Comprovar rotació correcta usando quaternions
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
                            obj.locked = true; //bloquejem els objectes
                            obj.GetComponent<Collider>().enabled = false; //desactivem els colliders dels objectes

                        }
                    }
                }
            }
            popUpManager.ShowPopUp("Has acabat el segon minijoc!", 2f); //pop up
            phasesManager.PasarAFase3(); //Pasar a la seguent fase
            

        }

    }
}
