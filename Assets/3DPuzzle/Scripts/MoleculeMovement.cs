using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MoleculeMovement : MonoBehaviour
{
    private float totalDuration = 2f;

    private AnimatedPipeController pipe;

    public void StartMovement(Transform objective, List<Transform> targets)
    {
        Sequence sequence = DOTween.Sequence();

        int steps = targets.Count + 1;
        float stepsDuration = totalDuration / steps;

        foreach (Transform target in targets)
        {
            sequence.Append(transform.DOMove(target.position, stepsDuration));
        }

        sequence.Append(transform.DOMove(objective.position, stepsDuration));

        sequence.OnComplete(() => 
        { 
            pipe.SetCanStartBloodFlow(true);
            Destroy(gameObject); 
        });
    }

    public void SetPipe(AnimatedPipeController pipe)
    {
        this.pipe = pipe;
    }
}
