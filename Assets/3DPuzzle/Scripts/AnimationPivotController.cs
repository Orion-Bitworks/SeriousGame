using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPivotController : MonoBehaviour
{
    [SerializeField] private int priority = 1;

    [Header("Pipe Parameters")]

    [SerializeField] GameObject pipePrefab;
    [SerializeField] private float spawnDistance = 15;

    public int GetPriority()
    {
        return priority;
    }

    public void StartAnimation()
    {
        Vector3 spawnPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + spawnDistance);

        GameObject newPipe = Instantiate(pipePrefab, transform.position, transform.rotation);
        newPipe.transform.SetParent(transform, true);
        newPipe.transform.localPosition = spawnPosition;

        Sequence sequence = DOTween.Sequence().SetAutoKill(false);
        sequence.AppendInterval(priority * 0.1f);
        sequence.Append(newPipe.transform.DOMove(transform.position, 0.9f).SetEase(Ease.OutBack));
        sequence.Join(newPipe.transform.DOLocalRotate(new Vector3(0f, 0f, 360f), 1.1f, RotateMode.FastBeyond360).SetEase(Ease.InOutQuad));
    }
}
