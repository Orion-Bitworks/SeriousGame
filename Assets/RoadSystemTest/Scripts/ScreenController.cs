using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    [SerializeField] GameObject screen;
    [SerializeField] Transform startingPosition;
    [SerializeField] Transform finalPosition;

    private void Start()
    {
        StartMovingIn();
    }

    void StartMovingIn()
    {
        screen.transform.DOMove(finalPosition.position, 1f).SetEase(Ease.OutBack);
    }

    public void StartMovingOut ()
    {
        screen.transform.DOMove(startingPosition.position, 2f).SetEase(Ease.OutCubic);
    }
}
