using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartPlaceholder : MonoBehaviour
{
    [Header("Right Plate")]
    [SerializeField] GameObject rightPlateTarget;
    [SerializeField] GameObject rightPlate;

    [Header("Left Plate")]
    [SerializeField] GameObject leftPlateTarget;
    [SerializeField] GameObject leftPlate;

    [Header("Animated Heart")]
    [SerializeField] GameObject animatedHeart;
    [SerializeField] Animator heartAnimator;

    public void ConnectPlates()
    {
        leftPlate.SetActive(true);
        rightPlate.SetActive(true);

        leftPlate.transform.DOMove(leftPlateTarget.transform.position, 1f).SetEase(Ease.OutBack, 0.4f);
        rightPlate.transform.DOMove(rightPlateTarget.transform.position, 1f).SetEase(Ease.OutBack, 0.4f).OnComplete(() =>
        {
            GameObject newAnimatedHeart = Instantiate(animatedHeart, transform.position, transform.rotation);
            ScoreManager.instance.RegisterHeartPlaceholder(newAnimatedHeart);
            Destroy(gameObject);
            DOVirtual.DelayedCall(4f, () =>
            {
                newAnimatedHeart.GetComponent<Animator>().SetTrigger("BeginPump");
            });
        });
    }
}
