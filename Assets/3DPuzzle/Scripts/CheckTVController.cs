using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum CHECKING_STATE { LOADING, CORRECT, WRONG }

public class CheckTVController : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] GameObject screenImage;
    [SerializeField] GameObject screen;

    [Header("Sprites")]
    [SerializeField] Animator screenAnimator;
    [SerializeField] Sprite glitchingSprite;
    [SerializeField] Sprite loadingSprite;
    [SerializeField] Sprite correctSprite;
    [SerializeField] Sprite wrongSprite;

    [Header("Parameters")]
    [SerializeField] Transform targetPosition;
    [SerializeField] Color loadingColor;
    [SerializeField] Color correctColor;
    [SerializeField] Color wrongColor;

    private Image image;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private bool isMoving = false;

    private Material screenMaterial;

    private Color startColor;

    private void Start()
    {
        startPosition = transform.position;
        endPosition = targetPosition.position;
        image = screenImage.GetComponent<Image>();

        screenMaterial = screen.GetComponent<MeshRenderer>().material;

        screenImage.SetActive(false);

        screenMaterial.EnableKeyword("_EMISSION");
        startColor = screenMaterial.GetColor("_EmissionColor");

        screenAnimator.enabled = true;
    }

    public void ShowTV()
    {
        isMoving = true;

        transform.DOLocalMove(endPosition, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            screenImage.SetActive(true);
            isMoving = false;
        });
    }

    public void HideTV()
    {
        //image.texture = glitchingSprite.texture;
        screenAnimator.SetTrigger("Glitch");
        DOVirtual.DelayedCall(0.1f, () =>
        {
            screenMaterial.DOColor(startColor, "_EmissionColor", 0.05f);
            screenImage.SetActive(false);
            transform.DOLocalMove(startPosition, 0.5f).SetEase(Ease.InBack);
        });
    }

    public void ChangeState(CHECKING_STATE state)
    {
        StartCoroutine(BeginChange(state));
    }

    private IEnumerator BeginChange(CHECKING_STATE state)
    {
        yield return new WaitUntil(() => !isMoving);

        //image.texture = glitchingSprite.texture;
        screenAnimator.SetTrigger("Glitch");

        DOVirtual.DelayedCall(0.1f, () =>
        {
            switch (state)
            {
                case CHECKING_STATE.LOADING:
                    //image.texture = loadingSprite.texture;
                    screenAnimator.SetTrigger("Loading");
                    screenMaterial.DOColor(loadingColor, "_EmissionColor", 0.05f);
                    break;
                case CHECKING_STATE.CORRECT:
                    //image.texture = correctSprite.texture;
                    screenAnimator.SetTrigger("Happy");
                    screenMaterial.DOColor(correctColor, "_EmissionColor", 0.05f);
                    break;
                case CHECKING_STATE.WRONG:
                    //image.texture = wrongSprite.texture;
                    screenAnimator.SetTrigger("Angry");
                    screenMaterial.DOColor(wrongColor, "_EmissionColor", 0.05f);
                    break;
            }
        });
    }
}
