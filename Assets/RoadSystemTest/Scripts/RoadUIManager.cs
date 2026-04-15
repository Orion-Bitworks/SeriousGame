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

    private void Start()
    {
        // Al iniciar, desactivamos el botón de stop
        stopButton.interactable = false;
    }

    /// <summary>
    /// Activamos el flujo en el sistema, y desactivamos todos los botones excepto el de stop, que lo activamos
    /// </summary>
    public void OnPlayButtonDown()
    {
        GameManager.Instance.Play();
        redoButton.interactable = false;
        undoButton.interactable = false;
        playButton.interactable = false;
        stopButton.interactable = true;
    }

    /// <summary>
    /// Paramos el flujo en el sistema, y activamos todos los botones excepto el de stop, que lo desactivamos
    /// </summary>
    public void OnStopButtonDown()
    {
        GameManager.Instance.Stop();
        redoButton.interactable = true;
        undoButton.interactable = true;
        playButton.interactable = true;
        stopButton.interactable = false;
    }

    /// <summary>
    /// Cuando se presiona el botón de Undo, se activa la acción de deshacer y la primera es instantánea (timer a 0)
    /// </summary>
    public void OnUndoButtonDown()
    {
        BuildController.Instance.isUndoing = true;
        BuildController.Instance.undoHoldTimer = 0f;
    }

    /// <summary>
    /// Cuando se suelta el botón de Undo, se desactiva la acción de deshacer
    /// </summary>
    public void OnUndoButtonUp()
    {
        BuildController.Instance.isUndoing = false;
    }

    /// <summary>
    /// Cuando se presiona el botón de Redo, se activa la acción de rehacer y la primera es instantánea (timer a 0)
    /// </summary>
    public void OnRedoButtonDown()
    {
        BuildController.Instance.isRedoing = true;
        BuildController.Instance.redoHoldTimer = 0f;
    }

    /// <summary>
    /// Cuando se suelta el botón de Redo, se desactiva la acción de rehacer
    /// </summary>
    public void OnRedoButtonUp()
    {
        BuildController.Instance.isRedoing = false;
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