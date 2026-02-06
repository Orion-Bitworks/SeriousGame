using UnityEngine;
using UnityEngine.UI;

public class ObjectSelector : MonoBehaviour
{
    public static SelectObject currentlySelected = null;

    [SerializeField]private Button rotateButton;

    static RotateObjects rotateObjectsInstance;

    public FasesMinigames minigamesPhasesInstance;

    private void Awake()
    {
        rotateButton.onClick.AddListener(rotatePiece);
    }

    private void rotatePiece()
    {

        if (currentlySelected == null)
        {
            Debug.Log("No se ha seleccionado ningun objeto");
            return;
        }

        rotateObjectsInstance = currentlySelected.GetComponentInParent<RotateObjects>(); //script rotateObjects

        if (rotateObjectsInstance == null)
        {
            Debug.LogError("RotateObjects NO encontrado en: " + currentlySelected.name);
            return;
        }

        Debug.Log("✅ Rotando: " + currentlySelected.name);

        if (minigamesPhasesInstance != null && minigamesPhasesInstance.fase1Root != null &&
            minigamesPhasesInstance.fase1Root.activeSelf)
        {
            rotateObjectsInstance.rotateObjectsMinigame1(currentlySelected);
            Debug.Log("Fase 1 - 180°");
        }
        else if (minigamesPhasesInstance != null && minigamesPhasesInstance.fase2Root != null &&
                 minigamesPhasesInstance.fase2Root.activeSelf)
        {
            rotateObjectsInstance.rotateObjectsMinigame2(currentlySelected);
            Debug.Log("Fase 2 - 90°");
        }
    }

}
