using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Minigame2 : MonoBehaviour
{
    public DragAndDropMinigame2[] draggagleVeins; // ← ADAPTADO
    int totalVeins;
    int placedVeins;

    public int correct = 0;

    public TextMeshProUGUI remainVeinstoDrag;

    [SerializeField] Button CheckButton;

    public FasesMinigames phasesManager;

    [SerializeField]
    private PopUp popUpManager;

    private bool popUpShown = false;

    [SerializeField]
    public TutorialManager tutorial;

    private void Awake()
    {
        CheckButton.onClick.AddListener(checkPlacementButton);
    }

    void Start()
    {
        totalVeins = draggagleVeins.Length;
        showInfo();
        tutorial.ShowTutorial(3);
        tutorial.MoveCarpetaMiniHeart();
    }

    public void objectsRemaining()
    {
        placedVeins = 0;

        foreach (DragAndDropMinigame2 obj in draggagleVeins) //
        {
            if (obj.placed)
                placedVeins++;
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

        foreach (DragAndDropMinigame2 obj in draggagleVeins) // ← ADAPTADO
        {
            if (obj.placed && obj.CurrentDropArea != null)
            {
                DropArea drop = obj.CurrentDropArea.GetComponent<DropArea>();

                if (drop != null && drop.valveType == obj.valveType)
                {
                    // Rotación relativa
                    Quaternion relativeRotation = Quaternion.Inverse(drop.transform.rotation) * obj.transform.rotation;
                    float angleDiff = Quaternion.Angle(relativeRotation, drop.requiredRotation);

                    if (angleDiff <= drop.rotationTolerance)
                        correct++;
                }
            }
        }

        Debug.Log("Objetos correctamente colocados: " + correct + " / " + draggagleVeins.Length);

        if (correct == draggagleVeins.Length)
        {
            foreach (DragAndDropMinigame2 obj in draggagleVeins) // ← ADAPTADO
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
                            obj.dragCollider.enabled = false; // ← ADAPTADO
                        }
                    }
                }
            }

            if (!popUpShown)
            {
                DialogManager.instance.Show("dialog_18_isgood");
                popUpManager.ShowPopUp("Has acabat el segon minijoc!", 2f);
                DialogManager.instance.Show("dialog_20");

                StartCoroutine(EndMinigame());
            }
        }
        else
        {
            DialogManager.instance.Show("dialog_19_isbad");
            popUpManager.ShowPopUp($"Només has fet: {correct}, torna-ho a intentar", 2f);
        }
    }

    IEnumerator EndMinigame()
    {
        yield return new WaitForSecondsRealtime(2f);
        popUpShown = true;
        phasesManager.PasarAFase3();
    }
}
