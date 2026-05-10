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
            // Caso 1 — No está colocada
            if (!obj.placed || obj.CurrentDropArea == null)
            {
                if (ObjectSelector.currentlySelected == obj.selectObj)
                {
                    ObjectSelector.currentlySelected.Deselect();
                    ObjectSelector.currentlySelected = null;
                }

                StartCoroutine(obj.FlashRed());
                continue;
            }

            DropArea drop = obj.CurrentDropArea.GetComponent<DropArea>();
            if (drop == null)
                continue;

            // Caso 2 — DropArea incorrecta
            if (drop.valveType != obj.valveType)
            {
                if (ObjectSelector.currentlySelected == obj.selectObj)
                {
                    ObjectSelector.currentlySelected.Deselect();
                    ObjectSelector.currentlySelected = null;
                }

                StartCoroutine(obj.FlashRed());

                continue;
            }

            // Comprobación de rotación
            Quaternion currentRot = obj.transform.localRotation;
            float angleDiff = Quaternion.Angle(currentRot, drop.requiredRotation);

            // Caso 3 — Rotación incorrecta
            if (angleDiff > drop.rotationTolerance)
            {
                if (ObjectSelector.currentlySelected == obj.selectObj)
                {
                    ObjectSelector.currentlySelected.Deselect();
                    ObjectSelector.currentlySelected = null;
                }

                StartCoroutine(obj.FlashRed());

                continue;
            }

            // ✔ Caso correcto
            correct++;
        }

		Debug.Log("Objetos correctamente colocados: " + correct + " / " + draggableValves.Length);

		fallos += draggableValves.Length - correct;

		// Caso éxito
		if (correct == draggableValves.Length)
		{
			//BLOQUEAR TODAS LAS PIEZAS (LO QUE TÚ QUIERES)
			foreach (DragAndDrop obj in draggableValves)
			{
				obj.locked = true;
				obj.GetComponent<Collider>().enabled = false;
			}

			AudioController.Instance.PlaySFX(SFX.ThreeD, (int)ThreeDSFX.ScreenCorrect);
			DialogManager.instance.Show("dialog_15_isgood");

			TerminarMinijuego();

			// Pasar a la siguiente fase con TODO bloqueado
			phasesManager.PasarAFase2();
		}
		else
		{
			AudioController.Instance.PlaySFX(SFX.ThreeD, (int)ThreeDSFX.ScreenError);
			DialogManager.instance.Show("dialog_16_isbad");
		}
	}

    private void TerminarMinijuego()
    {
        int tiempo = timer.Stop();

        GameParametersMDB.Instance.SaveMinigameData("MinijuegoCorazon1", tiempo, intentos, movimientos, fallos);
    }
}
