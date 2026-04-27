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

    public int correct = 0; //Aciertos

    public TextMeshProUGUI remainVeinstoDrag;

    [SerializeField] Button CheckButton; //Boto per comprovar

    public FasesMinigames phasesManager; //Instancia del script fasesMinigames

    private bool popUpShown = false;

	[SerializeField]
	public TutorialManager tutorial;


	private void Awake()
    {
        CheckButton.onClick.AddListener(checkPlacementButton);
    }
    void Start()
    {
        totalVeins = draggagleVeins.Length; //el total de objetos son la cantidad de objetos que haya en el array
        showInfo();
        tutorial.ShowTutorial(3);
		tutorial.MoveCarpetaMiniHeart();

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
            if (obj.placed && obj.CurrentDropArea != null)
            {
                DropArea drop = obj.CurrentDropArea.GetComponent<DropArea>();
                if (drop != null && drop.valveType == obj.valveType)
                {
                    //float angleDiff = Quaternion.Angle(obj.transform.rotation, drop.requiredRotation);

                    // Rotación de la pieza relativa a la DropArea
                    Quaternion relativeRotation = Quaternion.Inverse(drop.transform.rotation) * obj.transform.rotation;

                    // Comparamos esa rotación relativa con la rotación relativa requerida
                    float angleDiff = Quaternion.Angle(relativeRotation, drop.requiredRotation);

                    if (angleDiff <= drop.rotationTolerance)
                    {
                        correct++;
                    }
                }
            }
        }

        Debug.Log("Objetos correctamente colocados: " + correct + " / " + draggagleVeins.Length);

        if (correct == draggagleVeins.Length) // Caso éxito
        {
            foreach (DragAndDrop obj in draggagleVeins)
            {
                if (obj.CurrentDropArea != null)
                {
                    DropArea drop = obj.CurrentDropArea.GetComponent<DropArea>();
                    if (drop != null && drop.valveType == obj.valveType)
                    {
                        //float angleDiff = Quaternion.Angle(obj.transform.rotation, drop.requiredRotation);

                        Quaternion relativeRotation = Quaternion.Inverse(drop.transform.rotation) * obj.transform.rotation;
                        float angleDiff = Quaternion.Angle(relativeRotation, drop.requiredRotation);

                        if (angleDiff <= drop.rotationTolerance)
                        {
                            obj.locked = true;
                            obj.GetComponent<Collider>().enabled = false;
                        }
                    }
                }
            }

            if (!popUpShown)
            {
                DialogManager.instance.Show("dialog_18_isgood");
				DialogManager.instance.Show("dialog_20");

				StartCoroutine(EndMinigame());                
            }
        }
        else // Caso fallo
        {
			DialogManager.instance.Show("dialog_19_isbad");
        }
    }

    IEnumerator EndMinigame()
    {
        yield return new WaitForSecondsRealtime(2f);
        popUpShown = true;
        phasesManager.PasarAFase3();
    }
}
