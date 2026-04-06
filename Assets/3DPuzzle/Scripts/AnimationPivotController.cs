using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPivotController : MonoBehaviour
{
    [SerializeField] private int priority = 1;

    [SerializeField] BloodAnimationController connection;

    [Header("Pipe Parameters")]

    [SerializeField] GameObject pipePrefab;
    private float spawnDistance = 60;
    [SerializeField] bool invertedPipeDirection = false;

    public int GetPriority()
    {
        return priority;
    }

    public void StartAnimation(float animationDelay)
    {
        Vector3 spawnPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + spawnDistance);

        GameObject newPipe = Instantiate(pipePrefab, transform.position, transform.rotation);
        newPipe.transform.SetParent(transform, true);
        newPipe.transform.localPosition = spawnPosition;

        Sequence sequence = DOTween.Sequence().SetAutoKill(false);
        //sequence.AppendInterval(priority * 0.2f);
        sequence.AppendInterval(animationDelay);
        sequence.Append(newPipe.transform.DOMove(transform.position, 1.1f).SetEase(Ease.OutBack, 0.5f));
        sequence.Join(newPipe.transform.DOLocalRotate(new Vector3(0f, 0f, 360f), 1.3f, RotateMode.FastBeyond360).SetEase(Ease.InOutQuad));
        sequence.AppendInterval(0.5f);
        sequence.OnComplete(() =>
        {
            if (newPipe.GetComponent<AnimatedPipeController>() != null)
            {
                newPipe.GetComponent<AnimatedPipeController>().StartAnimation(invertedPipeDirection);
            }
            else
            {
                Debug.Log("No hay animated pipe controller");
            }

            StartCoroutine(StartBloodFlow(newPipe.GetComponent<AnimatedPipeController>()));
        });
    }

    public IEnumerator StartBloodFlow(AnimatedPipeController pipe)
    {
        yield return new WaitUntil(() => pipe.CanStartBloodFlow());

        if (connection != null)
        {
            connection.CheckBloodFlow();
        }
    }
}
