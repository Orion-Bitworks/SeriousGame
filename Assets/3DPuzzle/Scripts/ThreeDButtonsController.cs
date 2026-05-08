using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum Button3DScreenType
{
    check = 0,
    tutorial = 1
}

public class ThreeDButtonsController : MonoBehaviour
{
    [SerializeField] GameObject checkButton;
    [SerializeField] GameObject tutorialButton;

    Tween checkRumbleTween, tutorialRumbleTween;

    public void RumbleButton(int button)
    {
        switch ((Button3DScreenType) button)
        {
            case Button3DScreenType.check:
                AudioController.Instance.PlaySFX(SFX.UI, (int)UISFX.TableButtons);
                StartShortRumble(checkButton, ref checkRumbleTween);
                break;
            case Button3DScreenType.tutorial:
                AudioController.Instance.PlaySFX(SFX.UI, (int)UISFX.TableButtons);
                StartShortRumble(tutorialButton, ref tutorialRumbleTween);
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
