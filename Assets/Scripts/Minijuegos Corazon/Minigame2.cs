using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class Minigame2 : MonoBehaviour
{
    public DragAndDropMinigame2[] draggagleVeins;
    int totalVeins;
    int placedVeins;

    public int correct = 0;

    [SerializeField] Button CheckButton;

    public FasesMinigames phasesManager;

    [SerializeField]
    public TutorialManager tutorial;

    private SessionTimer timer;
    private int intentos;
    public int movimientos;
    private int fallos;

    [SerializeField] ScreenButtonsController screenButtonsController;

    private void Awake()
    {
        CheckButton.onClick.AddListener(checkPlacementButton);
    }

    void Start()
    {
        totalVeins = draggagleVeins.Length;
        timer = new SessionTimer();
        timer.Start();
        intentos = 0;
        movimientos = 0;
        fallos = 0;
        tutorial.ShowTutorial(3);
        tutorial.MoveCarpetaMiniHeart();
    }

    public void objectsRemaining()
    {
        placedVeins = 0;

        foreach (DragAndDropMinigame2 obj in draggagleVeins)
        {
            if (obj.placed)
                placedVeins++;
        }
    }

    public void checkPlacementButton()
    {
        intentos++;

        AudioController.Instance.PlaySFX(SFX.UI, (int)UISFX.TableButtons);
        screenButtonsController.RumbleButton(ButtonScreenType.check);

        correct = 0;

        Debug.Log("────────────── VALIDACIÓN MINIJUEGO 2 ──────────────");

        foreach (DragAndDropMinigame2 obj in draggagleVeins)
        {
            // No está colocada
            if (!obj.placed || obj.CurrentDropArea == null)
            {
                Debug.LogWarning($"❌ {obj.name} NO está colocada en ninguna DropArea.");

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
            {
                Debug.LogError($"⚠ {obj.name} tiene una CurrentDropArea sin DropArea script.");
                continue;
            }

            // Debug de DropArea asignada
            Debug.Log($"{obj.name} está usando DropArea: {drop.name}");

            // Comprobación de tipo
            if (drop.valveType != obj.valveType)
            {
                Debug.LogWarning($"❌ {obj.name} está en la DropArea equivocada ({drop.name}). " +
                                 $"Tipo requerido: {obj.valveType}, tipo DropArea: {drop.valveType}");

                if (ObjectSelector.currentlySelected == obj.selectObj)
                {
                    ObjectSelector.currentlySelected.Deselect();
                    ObjectSelector.currentlySelected = null;
                }

                StartCoroutine(obj.FlashRed());

                continue;
            }

            // Comprobación de rotación
            Quaternion relativeRotation = Quaternion.Inverse(drop.transform.rotation) * obj.transform.rotation;
            float angleDiff = Quaternion.Angle(relativeRotation, drop.requiredRotation);

            Debug.Log($"{obj.name} → Rot actual: {obj.transform.rotation.eulerAngles} | " +
                      $"Rot requerida: {drop.requiredEulerAngles} | " +
                      $"Diff: {angleDiff}° (tol: {drop.rotationTolerance}°)");

            if (angleDiff <= drop.rotationTolerance)
            {
                Debug.Log($"✔ {obj.name} está correctamente colocada.");
                correct++;
            }
            else
            {
                Debug.LogWarning($"❌ {obj.name} está mal rotada. Diferencia: {angleDiff}°");
                
                if (ObjectSelector.currentlySelected == obj.selectObj)
                {
                    ObjectSelector.currentlySelected.Deselect();
                    ObjectSelector.currentlySelected = null;
                }

                StartCoroutine(obj.FlashRed());
            }
        }

        Debug.Log($"RESULTADO FINAL → {correct} / {draggagleVeins.Length} correctas");

        fallos += draggagleVeins.Length - correct;

        // Caso éxito
        if (correct == draggagleVeins.Length)
        {
            foreach (DragAndDropMinigame2 obj in draggagleVeins)
            {
                if (obj.CurrentDropArea != null)
                {
                    DropArea drop = obj.CurrentDropArea.GetComponent<DropArea>();

                    if (drop != null && drop.valveType == obj.valveType)
                    {
                        Quaternion relativeRotation = Quaternion.Inverse(drop.transform.rotation) * obj.transform.rotation;
                        float angleDiff = Quaternion.Angle(relativeRotation, drop.requiredRotation);

                        if (angleDiff <= drop.rotationTolerance)
                        {
                            obj.locked = true;
                            obj.dragCollider.enabled = false;
                        }
                    }
                }
            }
            AudioController.Instance.PlaySFX(SFX.ThreeD, (int)ThreeDSFX.ScreenCorrect);
            DialogManager.instance.Show("dialog_18_isgood");
            DialogManager.instance.Show("dialog_20");

            StartCoroutine(EndMinigame());
        }
        else
        {
            AudioController.Instance.PlaySFX(SFX.ThreeD, (int)ThreeDSFX.ScreenError);
            DialogManager.instance.Show("dialog_19_isbad");
        }
    }

    IEnumerator EndMinigame()
    {
        yield return new WaitForSecondsRealtime(2f);
        TerminarMinijuego();
        phasesManager.PasarAFase3();
    }

    private void TerminarMinijuego()
    {
        int tiempo = timer.Stop();

        GameParametersMDB.Instance.SaveMinigameData("MinijuegoCorazon2", tiempo, intentos, movimientos, fallos);
    }
}