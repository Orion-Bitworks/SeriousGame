using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ObjectSelector : MonoBehaviour
{
    public static SelectObject currentlySelected = null;

    [SerializeField]private Button rotateButton; //Boton de rotar
    [SerializeField]private Button checkButton; //Boton de rotar

    static RotateObjects rotateObjectsInstance; //Instancia del script RotateObjects

    public FasesMinigames minigamesPhasesInstance; //Instancia del script FasesMinigames

    private DragAndDrop dragAndDropInstance;

    Controls controls;

    [SerializeField] Minigame1 minigame1;
    [SerializeField] Minigame2 minigame2;

    [SerializeField] ScreenButtonsController screenButtonsController;

    bool wasDialogActive = false;

    private void Awake()
    {
        rotateButton.onClick.AddListener(rotatePiece); //Listener
        dragAndDropInstance = GetComponent<DragAndDrop>();
        
        controls = new Controls();
    }

    private void Start()
    {
        controls.Enable();

        controls.InMiniGame.Rotate.performed += OnRotate;
        controls.InMiniGame.Check.performed += OnCheck;
    }

    private void Update()
    {
        if (DialogManager.IsDialogActive && !wasDialogActive)
        {
            controls.Disable();
            wasDialogActive = true;
        }
        else if (!DialogManager.IsDialogActive && wasDialogActive)
        {
            controls.Enable();
            wasDialogActive = false;
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void OnRotate(InputAction.CallbackContext ctx)
    {
        StartCoroutine(InstantFlashButton(rotateButton));
        rotatePiece();
    }

    private void OnCheck(InputAction.CallbackContext ctx)
    {
        StartCoroutine(InstantFlashButton(checkButton));
        CheckMinigame();
    }

    private void CheckMinigame()
    {
        if (minigamesPhasesInstance != null && minigamesPhasesInstance.fase1Root != null && minigamesPhasesInstance.fase1Root.activeSelf)
        {
            minigame1.checkPlacementButton();
        }
        else if (minigamesPhasesInstance != null && minigamesPhasesInstance.fase2Root != null && minigamesPhasesInstance.fase2Root.activeSelf)
        {
            minigame2.checkPlacementButton();
        }
    }

    private void rotatePiece()
    {
        AudioController.Instance.PlaySFX(SFX.UI, (int)UISFX.TableButtons);
        screenButtonsController.RumbleButton(ButtonScreenType.rotate);

        if (currentlySelected == null)
        {
            Debug.Log("No se ha seleccionado ningun objeto");
            return;
        }

        if (dragAndDropInstance != null && dragAndDropInstance.locked) 
        { 
            Debug.Log("Este objeto está bloqueado y no se puede rotar."); 
            return; 
        }

        rotateObjectsInstance = currentlySelected.GetComponentInParent<RotateObjects>(); //script rotateObjects

        if (rotateObjectsInstance == null)
        {
            Debug.LogError("RotateObjects NO encontrado en: " + currentlySelected.name);
            return;
        }


        if (minigamesPhasesInstance != null && minigamesPhasesInstance.fase1Root != null && minigamesPhasesInstance.fase1Root.activeSelf)
        {
            rotateObjectsInstance.rotateObjectsMinigame1(currentlySelected);
            Debug.Log("Fase 1 - 180°");
        }
        else if (minigamesPhasesInstance != null && minigamesPhasesInstance.fase2Root != null && minigamesPhasesInstance.fase2Root.activeSelf)
        {
            rotateObjectsInstance.rotateObjectsMinigame2(currentlySelected);
            Debug.Log("Fase 2 - 90°");
        }
    }

    private IEnumerator InstantFlashButton(Button button)
    {
        if (button == null) yield break;

        SetButtonPressed(button);
        yield return new WaitForSeconds(.1f);
        SetButtonNormal(button);
    }

    /// <summary>
    /// Le cambia el color al botón para que parezca que está siendo pulsado
    /// </summary>
    /// <param name="button">Botón a modificar visualmente</param>
    void SetButtonPressed(Button button)
    {
        if (button == null) return;

        var colors = button.colors;
        button.targetGraphic.color = colors.pressedColor;
    }

    /// <summary>
    /// Le cambia el color al botón para que parezca que ya no se está pulsando
    /// </summary>
    /// <param name="button">Botón a modificar visualmente</param>
    void SetButtonNormal(Button button)
    {
        if (button == null) return;

        var colors = button.colors;
        button.targetGraphic.color = colors.normalColor;
    }
}
