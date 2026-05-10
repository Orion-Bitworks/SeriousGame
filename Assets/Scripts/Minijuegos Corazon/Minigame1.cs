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

    private bool popUpShown = false;

    [SerializeField]
    public TutorialManager tutorial;

    Controls controls;

    private SessionTimer timer;
    private int intentos;
    public int movimientos;
    private int fallos;

    private void Awake()
    {
        CheckButton.onClick.AddListener(checkPlacementButton); //Listener
    }

    void Start()
    {
        totalValves = draggableValves.Length; //el total de objetos son la cantidad de objetos que haya en el array
        timer = new SessionTimer();
        timer.Start();
        intentos = 0;
        movimientos = 0;
        fallos = 0;
        showInfo();

        tutorial.ShowTutorial(2);
        tutorial.MoveCarpetaMiniHeart();
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
        remainObjectsToDrag.text = placedValves.ToString() + " / " + totalValves.ToString(); 
    }

    public void checkPlacementButton()
    {
        intentos++;
        correct = 0;

        foreach (DragAndDrop obj in draggableValves)
        {
            if (obj.placed && obj.CurrentDropArea != null)
            {
                DropArea drop = obj.CurrentDropArea.GetComponent<DropArea>();

                if (drop != null && drop.valveType == obj.valveType)
                {
                    Quaternion currentRot = obj.transform.localRotation;
                    float angleDiff = Quaternion.Angle(currentRot, drop.requiredRotation);

                    if (angleDiff <= drop.rotationTolerance)
                        correct++;
                }
            }
        }

        Debug.Log("Objetos correctamente colocados: " + correct + " / " + draggableValves.Length);

        fallos += draggableValves.Length - correct;

        if (correct == draggableValves.Length)
        {
            // 🔒🔒🔒 BLOQUEAR TODAS LAS VÁLVULAS 🔒🔒🔒
            foreach (DragAndDrop obj in draggableValves)
            {
                obj.locked = true;
                obj.GetComponent<Collider>().enabled = false;
            }

            if (!popUpShown)
            {
                DialogManager.instance.Show("dialog_15_isgood");
                popUpShown = true;
                TerminarMinijuego();

                // Pasar al minijuego 2 con válvulas ya bloqueadas
                phasesManager.PasarAFase2();
            }
            else
            {
                DialogManager.instance.Show("dialog_16_isbad");
            }
        }
    }

    private void TerminarMinijuego()
    {
        int tiempo = timer.Stop();

        GameParametersMDB.Instance.SaveMinigameData("MinijuegoCorazon1", tiempo, intentos, movimientos, fallos);
    }
}
