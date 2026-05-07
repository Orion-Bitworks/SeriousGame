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

        //int steps = targets.Count + 1;
        //float stepsDuration = totalDuration / steps;

        List<Vector3> steps = new List<Vector3>();

        foreach (Transform transform in targets)
        {
            steps.Add(transform.position);
        }

        steps.Add(objective.position);

        float totalDistance = 0f;
        Vector3 currentPos = transform.position;

        foreach (Vector3 step in steps)
        {
            totalDistance += Vector3.Distance(currentPos, step);
            currentPos = step;
        }

        currentPos = transform.position;

        foreach (Vector3 step in steps)
        {
            float distance = Vector3.Distance(currentPos, step);
            float duration = (distance / totalDistance) * totalDuration;

            sequence.Append(transform.DOMove(step, duration).SetEase(Ease.Linear));
            currentPos = step;
        }

        sequence.OnComplete(() => 
        {
            if (this == null || transform == null)
            {
                return;
            }

            if (pipe != null)
            {
                AudioController.Instance.PlayHeartbeatOnce();
            }

            DOTween.Kill(transform);
            AudioController.Instance.PlaySFX(SFX.Heart, (int)HeartSFX.ParticleInOut);
            Destroy(gameObject); 
        });

        sequence.OnKill(() =>
        {
        });
    }

    public void SetPipe(AnimatedPipeController pipe)
    {
        this.pipe = pipe;
    }
}
