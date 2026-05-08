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

    [SerializeField] ScreenButtonsController screenButtonsController;

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
    }

    public void checkPlacementButton()
    {
        intentos++;

        AudioController.Instance.PlaySFX(SFX.UI, (int)UISFX.TableButtons);
        screenButtonsController.RumbleButton(ButtonScreenType.check);

        correct = 0;

        foreach (DragAndDrop obj in draggableValves)
        {
            if (obj.placed && obj.CurrentDropArea != null)
            {
                DropArea drop = obj.CurrentDropArea.GetComponent<DropArea>();

                if (drop != null)
                {
                    if (drop.valveType == obj.valveType)
                    {
                        Quaternion currentRot = obj.transform.localRotation; //comprobamos la rotacion local del transform
                        float angleDiff = Quaternion.Angle(currentRot, drop.requiredRotation);
                        if (angleDiff <= drop.rotationTolerance)
                        {
                            correct++;
                        }
                    }
                }
            }
        }

        Debug.Log("Objetos correctamente colocados: " + correct + " / " + draggableValves.Length);

        fallos += draggableValves.Length - correct;

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
                            obj.locked = true; //bloqueja el objecte
                            obj.GetComponent<Collider>().enabled = false; //desactiva el collider

                        }
                    }
                }
            }

            if (!popUpShown)
            {
                DialogManager.instance.Show("dialog_15_isgood");
                popUpShown = true;
                TerminarMinijuego();
                phasesManager.PasarAFase2(); //pasa a la siguiente fase
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
