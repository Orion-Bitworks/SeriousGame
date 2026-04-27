using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestiona toda la UI de la pantalla de carreteras
/// </summary>
public class RoadUIManager : MonoBehaviour
{
    [SerializeField] GameObject undoShortcutPanel;                  // Referencia al panel con el shortcut de Undo, para cuando haces hover en el botón
    [SerializeField] GameObject redoShortcutPanel;                  // Referencia al panel con el shortcut de Redo, para cuando haces hover en el botón
    [SerializeField] Button redoButton;                             // Referencia al botón de Redo
    [SerializeField] Button undoButton;                             // Referencia al botón de Undo
    [SerializeField] Button playButton;                             // Referencia al botón de Play
    [SerializeField] Button stopButton;                             // Referencia al botón de Stop
    [SerializeField] Button leftButton;                             // Referencia al botón de Left
    [SerializeField] Button rotateButton;                           // Referencia al botón de Rotate
    [SerializeField] Button rightButton;                            // Referencia al botón de Right
    [SerializeField] Button x1Button;                               // Referencia al botón de x1
    [SerializeField] Button x2Button;                               // Referencia al botón de x2
    [SerializeField] Button x3Button;                               // Referencia al botón de x3

    [SerializeField] TableButtonsController tableButtonsController;
    [SerializeField] MiniScreenController miniScreen;

    /// <summary>
    /// Activamos el flujo en el sistema, y desactivamos todos los botones excepto el de stop, que lo activamos
    /// </summary>
    public void OnPlayButtonDown()
    {
        GameManager.Instance.Play();
        redoButton.interactable = false;
        undoButton.interactable = false;
        playButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(true);
        AudioController.Instance.PlaySFX(SFX.UI, (int)UISFX.TableButtons);
        tableButtonsController.RumbleButton(ButtonType.play);
    }

    /// <summary>
    /// Paramos el flujo en el sistema, y activamos todos los botones excepto el de stop, que lo desactivamos
    /// </summary>
    public void OnStopButtonDown()
    {
        GameManager.Instance.Stop();
        redoButton.interactable = true;
        undoButton.interactable = true;
        stopButton.gameObject.SetActive(false);
        playButton.gameObject.SetActive(true);
        AudioController.Instance.PlaySFX(SFX.UI, (int)UISFX.TableButtons);
        tableButtonsController.RumbleButton(ButtonType.play);
    }

    public void OnLeftButtonDown()
    {
        BuildController.Instance.ChangeObject(false);
        AudioController.Instance.PlaySFX(SFX.UI, (int)UISFX.ScreenTouch);
        miniScreen.RumbleLeft();
    }

    public void OnRotateButtonDown()
    {
        BuildController.Instance.RotateGhostAndPreview();
        AudioController.Instance.PlaySFX(SFX.UI, (int)UISFX.ScreenTouch);
        miniScreen.RumbleDouble();
    }

    public void OnRightButtonDown()
    {
        BuildController.Instance.ChangeObject(true);
        AudioController.Instance.PlaySFX(SFX.UI, (int)UISFX.ScreenTouch);
        miniScreen.RumbleRight();
    }

    public void X1ButtonDown()
    {
        x1Button.gameObject.SetActive(false);
        x2Button.gameObject.SetActive(true);
        GameManager.Instance.velocityMultiplier = 2;
        AudioController.Instance.PlaySFX(SFX.UI, (int)UISFX.TableButtons);
        tableButtonsController.RumbleButton(ButtonType.multiplier);
    }

    public void X2ButtonDown()
    {
        x2Button.gameObject.SetActive(false);
        x3Button.gameObject.SetActive(true);
        GameManager.Instance.velocityMultiplier = 3;
        AudioController.Instance.PlaySFX(SFX.UI, (int)UISFX.TableButtons);
        tableButtonsController.RumbleButton(ButtonType.multiplier);
    }

    public void X3ButtonDown()
    {
        x3Button.gameObject.SetActive(false);
        x1Button.gameObject.SetActive(true);
        GameManager.Instance.velocityMultiplier = 1;
        AudioController.Instance.PlaySFX(SFX.UI, (int)UISFX.TableButtons);
        tableButtonsController.RumbleButton(ButtonType.multiplier);
    }

    /// <summary>
    /// Cuando se presiona el botón de Undo, se activa la acción de deshacer y la primera es instantánea (timer a 0)
    /// </summary>
    public void OnUndoButtonDown()
    {
        BuildController.Instance.isUndoing = true;
        BuildController.Instance.undoHoldTimer = 0f;

        AudioController.Instance.PlaySFX(SFX.UI, (int)UISFX.TableButtons);
        tableButtonsController.RumbleButton(ButtonType.undo);
    }

    /// <summary>
    /// Cuando se suelta el botón de Undo, se desactiva la acción de deshacer
    /// </summary>
    public void OnUndoButtonUp()
    {
        BuildController.Instance.isUndoing = false;

        tableButtonsController.StopRumbleButton(ButtonType.undo);
    }

    /// <summary>
    /// Cuando se presiona el botón de Redo, se activa la acción de rehacer y la primera es instantánea (timer a 0)
    /// </summary>
    public void OnRedoButtonDown()
    {
        BuildController.Instance.isRedoing = true;
        BuildController.Instance.redoHoldTimer = 0f;

        AudioController.Instance.PlaySFX(SFX.UI, (int)UISFX.TableButtons);
        tableButtonsController.RumbleButton(ButtonType.redo);
    }

    /// <summary>
    /// Cuando se suelta el botón de Redo, se desactiva la acción de rehacer
    /// </summary>
    public void OnRedoButtonUp()
    {
        BuildController.Instance.isRedoing = false;

        tableButtonsController.StopRumbleButton(ButtonType.redo);
    }

    /// <summary>
    /// Cuando se hace hover al botón de Undo, se activa el panel del shortcut
    /// </summary>
    public void OnUndoHoverEnter()
    {
        //undoShortcutPanel.SetActive(true);
    }

    /// <summary>
    /// Cuando se deja de hacer hover al botón de Undo, se desactiva el panel del shortcut
    /// </summary>
    public void OnUndoHoverExit()
    {
        //undoShortcutPanel.SetActive(false);
    }

    /// <summary>
    /// Cuando se hace hover al botón de Redo, se activa el panel del shortcut
    /// </summary>
    public void OnRedoHoverEnter()
    {
        //redoShortcutPanel.SetActive(true);
    }

    /// <summary>
    /// Cuando se deja de hacer hover al botón de Redo, se desactiva el panel del shortcut
    /// </summary>
    public void OnRedoHoverExit()
    {
        //redoShortcutPanel.SetActive(false);
    }
}