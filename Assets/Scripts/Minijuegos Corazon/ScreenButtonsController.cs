using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum ButtonScreenType
{
    rotate = 0,
    check = 1
}

public class ScreenButtonsController : MonoBehaviour
{
    [SerializeField] GameObject rotateButton;
    [SerializeField] GameObject checkButton;

    Tween rotateRumbleTween, checkRumbleTween;

    public void RumbleButton(ButtonScreenType button)
    {
        switch (button)
        {
            case ButtonScreenType.rotate:
                StartShortRumble(rotateButton, ref rotateRumbleTween);
                break;
            case ButtonScreenType.check:
                StartShortRumble(checkButton, ref checkRumbleTween);
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
}
