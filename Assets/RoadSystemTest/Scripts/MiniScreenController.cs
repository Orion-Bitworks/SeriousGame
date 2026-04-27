using DG.Tweening;
using UnityEngine;

public class MiniScreenController : MonoBehaviour
{
    Tween rumbleTween;
    Vector3 initialRot;

    void Awake()
    {
        initialRot = transform.localEulerAngles;
    }

    public void RumbleLeft()
    {
        StartRumble(new Vector3(0, -3f, 0));   // rotación hacia la izquierda
    }

    public void RumbleRight()
    {
        StartRumble(new Vector3(0, 3f, 0));  // rotación hacia la derecha
    }

    public void RumbleDouble()
    {
        if (rumbleTween != null) rumbleTween.Kill();

        rumbleTween = DOTween.Sequence()
            // izquierda
            .Append(transform.DOLocalRotate(initialRot + new Vector3(0, 3f, 0), 0.05f))
            // derecha
            .Append(transform.DOLocalRotate(initialRot + new Vector3(0, -3f, 0), 0.05f))
            // centro
            .Append(transform.DOLocalRotate(initialRot, 0.05f))
            .OnKill(() => transform.localEulerAngles = initialRot);
    }

    void StartRumble(Vector3 rotOffset)
    {
        if (rumbleTween != null)
        {
            rumbleTween.Kill();
            rumbleTween = null;
        }

        rumbleTween = DOTween.Sequence()
            .Append(transform.DOLocalRotate(initialRot + rotOffset, 0.05f))
            .Append(transform.DOLocalRotate(initialRot, 0.05f))
            .OnKill(() => transform.localEulerAngles = initialRot);
    }
}