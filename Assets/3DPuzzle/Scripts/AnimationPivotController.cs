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

    private Vector3 originalPipePos;

    private AnimatedPipeController pipeController;

    private Sequence entrySequence;
    private Sequence exitSequence;

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
        originalPipePos = spawnPosition;
        pipeController = newPipe.GetComponent<AnimatedPipeController>();

        entrySequence = DOTween.Sequence().SetAutoKill(true);
        //sequence.AppendInterval(priority * 0.2f);
        entrySequence.AppendInterval(animationDelay);

        MakeSequence(entrySequence, newPipe.transform);

        entrySequence.AppendInterval(0.5f);
        entrySequence.OnComplete(() =>
        {
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
                sequence.Join(target.DOMoveZ(transform.position.z, 0.8f).SetEase(Ease.OutQuad));
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

        StartCoroutine(ShowPlayState());
    }

    public void StartExitAnimation(float animationDelay)
    {
        if (pipeController == null)
        {
            return;
        }

        pipeController.StopSpawning();

        AnimatedPipeController pipe = pipeController;

        DOTween.Kill(pipeController.transform);
        DOTween.Kill(pipeController.gameObject);

        exitSequence = DOTween.Sequence().SetAutoKill(true);
        exitSequence.AppendInterval(animationDelay);

        switch (animationType)
        {
            case ANIMATION_TYPE.LINEAR_SPINNING:
                exitSequence.Append(pipe.transform.DOLocalMove(originalPipePos, 1.1f).SetEase(Ease.InBack, 0.5f));
                exitSequence.Join(pipe.transform.DOLocalRotate(new Vector3(0f, 0f, 360f), 1.3f, RotateMode.FastBeyond360).SetEase(Ease.InOutQuad));
                break;
            case ANIMATION_TYPE.L_SHAPE:
                exitSequence.Append(pipe.transform.DOLocalMoveY(originalPipePos.y, 0.2f).SetEase(Ease.InOutBack));
                exitSequence.AppendInterval(0.1f);
                exitSequence.Append(pipe.transform.DOLocalMoveX(originalPipePos.x, 0.8f).SetEase(Ease.InQuad));
                exitSequence.Join(pipe.transform.DOLocalMoveZ(originalPipePos.z, 0.8f).SetEase(Ease.InQuad));
                break;
        }

        exitSequence.OnComplete(() => 
        {
            if (pipeController != null)
            {
                Destroy(pipeController.gameObject);
            }

            started = false;
            pipeController = null;
        });
    }

    public IEnumerator StartBloodFlow(AnimatedPipeController pipeController)
    {
        yield return new WaitUntil(() => pipeController.CanStartBloodFlow());

        if (connection != null)
        {
            connection.CheckBloodFlow();
        }
    }

    public IEnumerator ShowPlayState()
    {
        yield return new WaitForSeconds(5f);
        ScoreManager.instance.CheckConnections();
    }
}
