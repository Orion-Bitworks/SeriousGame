using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ANIMATION_TYPE { LINEAR_SPINNING, L_SHAPE}

public class AnimationPivotController : MonoBehaviour
{
    [SerializeField] private int priority = 1;

    [SerializeField] BloodAnimationController connection;

    [Header("Pipe Parameters")]

    [SerializeField] GameObject pipePrefab;
    [SerializeField] private float spawnDistanceX = 0;
    [SerializeField] private float spawnDistanceY = 0;
    [SerializeField] private float spawnDistanceZ = 60;
    [SerializeField] bool invertedPipeDirection = false;
    [SerializeField] private ANIMATION_TYPE animationType;

    private AnimatedPipeController pipeController;

    bool started = false;

    public int GetPriority()
    {
        return priority;
    }

    public void StartAnimation(float animationDelay)
    {
        Vector3 spawnPosition = new Vector3(transform.localPosition.x + spawnDistanceX, transform.localPosition.y + spawnDistanceY, transform.localPosition.z + spawnDistanceZ);

        GameObject newPipe = Instantiate(pipePrefab, transform.position, transform.rotation);
        newPipe.transform.SetParent(transform, true);
        newPipe.transform.localPosition = spawnPosition;

        Sequence sequence = DOTween.Sequence().SetAutoKill(false);
        //sequence.AppendInterval(priority * 0.2f);
        sequence.AppendInterval(animationDelay);

        MakeSequence(sequence, newPipe.transform);

        sequence.AppendInterval(0.5f);
        sequence.OnComplete(() =>
        {
            pipeController = newPipe.GetComponent<AnimatedPipeController>();

            if (pipeController != null && !invertedPipeDirection)
            {
                pipeController.StartAnimation(invertedPipeDirection);
                StartCoroutine(StartBloodFlow(pipeController));
            }
            else
            {
                Debug.Log("No hay animated pipe controller");
            }
        });
    }

    public void MakeSequence(Sequence sequence, Transform target)
    {
        switch (animationType)
        {
            case ANIMATION_TYPE.LINEAR_SPINNING:
                sequence.Append(target.DOMove(transform.position, 1.1f).SetEase(Ease.OutBack, 0.5f));
                sequence.Join(target.DOLocalRotate(new Vector3(0f, 0f, 360f), 1.3f, RotateMode.FastBeyond360).SetEase(Ease.InOutQuad));
                break;
            case ANIMATION_TYPE.L_SHAPE:
                sequence.Append(target.DOMoveX(transform.position.x, 0.8f).SetEase(Ease.OutQuad));
                sequence.AppendInterval(0.1f);
                sequence.Append(target.DOMoveY(transform.position.y, 0.2f).SetEase(Ease.InOutBack));
                break;
        }
    }

    public void StartInvertedAnimation()
    {
        if (pipeController != null && !started)
        {
            started = true;

            Sequence sequence = DOTween.Sequence().AppendInterval(1f);
            sequence.OnComplete(() => { pipeController.StartAnimation(invertedPipeDirection); });
        }
    }

    public IEnumerator StartBloodFlow(AnimatedPipeController pipeController)
    {
        yield return new WaitUntil(() => pipeController.CanStartBloodFlow());

        if (connection != null)
        {
            connection.CheckBloodFlow();
        }
    }
}
