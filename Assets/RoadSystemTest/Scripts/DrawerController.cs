using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class DrawerController : MonoBehaviour
{
    [SerializeField] GameObject drawer;
    [SerializeField] Collider drawerCollider;
    [SerializeField] Transform openPosition;
    [SerializeField] Transform closedPosition;

    bool drawerOut = false;
    bool isMoving = false;
    Coroutine closeRoutine;
    Tween rumbleTween;

    private void Update()
    {
        if (GameManager.Instance.isPlaying) return;

        if (IsMouseOverHandle() && !drawerOut)
        {
            // Si hacemos click encima del corazón del cajón, empezamos a arrastrar el corazón real
            if (Input.GetMouseButtonDown(0))
            {
                StartMovingOut();
                isMoving = true;
            }

            if (!isMoving)
                StartRumble();
        }
        else
        {
            StopRumble();
        }

        if (!IsMouseOverDrawer() && drawerOut)
        {
            if (closeRoutine == null)
            {
                closeRoutine = StartCoroutine(DelayedStartMovingIn());
            }
        }
        else
        {
            if (closeRoutine != null)
            {
                StopCoroutine(closeRoutine);
                closeRoutine = null;
            }
        }
    }

    void StartMovingOut()
    {
        drawer.transform.DOMove(openPosition.position, 1f).SetEase(Ease.OutBack).OnComplete(() => drawerOut = true);
    }

    IEnumerator DelayedStartMovingIn()
    {
        yield return new WaitForSecondsRealtime(3f);
        StartMovingIn();
    }

    void StartMovingIn()
    {
        drawer.transform.DOMove(closedPosition.position, 2f).SetEase(Ease.OutCubic).OnComplete(() => { drawerOut = false; isMoving = false; });
    }

    void StartRumble()
    {
        if (rumbleTween != null && rumbleTween.IsActive()) return;

        rumbleTween = drawer.transform.DOLocalMoveX(
            drawer.transform.localPosition.x + 0.05f,
            0.4f
        )
        .SetEase(Ease.InOutSine)
        .SetLoops(-1, LoopType.Yoyo)
        .OnKill(() => drawer.transform.position = closedPosition.position);
    }

    void StopRumble()
    {
        if (rumbleTween != null)
        {
            rumbleTween.Kill();
            rumbleTween = null;
        }
    }

    bool IsMouseOverHandle()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform;
    }

    bool IsMouseOverDrawer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out RaycastHit hit) && hit.transform == drawerCollider.transform;
    }
}
