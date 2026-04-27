using System.Collections;
using DG.Tweening;
using UnityEngine;

public enum ButtonType
{
    undo = 0,
    redo = 1,
    play = 2,
    multiplier = 3
}

public class TableButtonsController : MonoBehaviour
{
    [SerializeField] GameObject undoButton;
    [SerializeField] GameObject redoButton;
    [SerializeField] GameObject playButton;
    [SerializeField] GameObject multiplierButton;

    Tween undoRumbleTween, redoRumbleTween, playRumbleTween, multiplierRumbleTween;

    public void RumbleButton(ButtonType button)
    {
        switch (button)
        {
            case ButtonType.undo:
                StartHoldRumble(undoButton, ref undoRumbleTween);
                break;
            case ButtonType.redo:
                StartHoldRumble(redoButton, ref redoRumbleTween);
                break;
            case ButtonType.play:
                StartShortRumble(playButton, ref playRumbleTween);
                break;
            case ButtonType.multiplier:
                StartShortRumble(multiplierButton, ref multiplierRumbleTween);
                break;
        }
    }

    public void StopRumbleButton (ButtonType button)
    {
        switch (button)
        {
            case ButtonType.undo:
                StopRumble(ref undoRumbleTween);
                break;
            case ButtonType.redo:
                StopRumble(ref redoRumbleTween);
                break;
        }
    }

    void StartShortRumble(GameObject button, ref Tween rumbleTween)
    {
        // Si había un tween anterior, lo matamos
        if (rumbleTween != null)
        {
            rumbleTween.Kill();
            rumbleTween = null;
        }

        Transform t = button.transform;
        Vector3 initialRot = t.localEulerAngles;

        rumbleTween = DOTween.Sequence()
            .Append(t.DOLocalRotate(initialRot + new Vector3(0, 3f, 0), 0.07f))
            .Append(t.DOLocalRotate(initialRot - new Vector3(0, 3f, 0), 0.07f))
            .Append(t.DOLocalRotate(initialRot, 0.07f).SetEase(Ease.OutQuad))
            .OnKill(() => t.localEulerAngles = initialRot);
    }

    void StartHoldRumble(GameObject button, ref Tween rumbleTween)
    {
        if (rumbleTween != null && rumbleTween.IsActive()) return;

        Transform t = button.transform;
        Vector3 initialRot = t.localEulerAngles;

        rumbleTween = t.DOLocalRotate(initialRot + new Vector3(0, 3f, 0), 0.07f)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo)
            .OnKill(() => t.localEulerAngles = initialRot);
    }

    void StopRumble(ref Tween rumbleTween)
    {
        if (rumbleTween != null)
        {
            rumbleTween.Kill();
            rumbleTween = null;
        }
    }
}