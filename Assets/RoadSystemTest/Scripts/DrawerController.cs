using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DrawerController : MonoBehaviour
{
    enum DrawerState { Closed, Opening, Open, AutoClosing, Closing }
    DrawerState state = DrawerState.Closed;

    [SerializeField] GameObject drawer;
    [SerializeField] Collider drawerCollider;
    [SerializeField] Transform openPosition;
    [SerializeField] Transform closedPosition;
    [SerializeField] Image arrow;

    Tween rumbleTween;
    Coroutine autoCloseRoutine;

    Color normalColor = Color.white;
    Color hoverColor = new Color(0.7f, 0.7f, 0.7f);
    Color pressedColor = new Color(0.5f, 0.5f, 0.5f);
    Color disabledColor = new Color(0.7843137f, 0.7843137f, 0.7843137f, 0.5019608f);

    void Update()
    {
        if (GameManager.Instance.isPlaying) return;

        bool overHandle = IsMouseOverHandle();
        bool overDrawer = IsMouseOverDrawer();

        UpdateArrowColor(overHandle);

        switch (state)
        {
            case DrawerState.Closed:
                HandleClosed(overHandle);
                break;

            case DrawerState.Opening:
                break;

            case DrawerState.Open:
                HandleOpen(overHandle, overDrawer);
                break;

            case DrawerState.AutoClosing:
                break;

            case DrawerState.Closing:
                break;
        }
    }

    // -------------------------
    // STATE: CLOSED
    // -------------------------
    void HandleClosed(bool overHandle)
    {
        if (overHandle)
        {
            StartRumble();

            if (Input.GetMouseButtonDown(0))
                StartOpening();
        }
        else
        {
            StopRumble();
        }
    }

    // -------------------------
    // STATE: OPEN
    // -------------------------
    void HandleOpen(bool overHandle, bool overDrawer)
    {
        if (overHandle && Input.GetMouseButtonDown(0))
        {
            StartClosing();
            return;
        }

        // Cerrar automáticamente si el ratón NO está sobre el cajón ni el handle
        if (!overDrawer && !overHandle && autoCloseRoutine == null)
            autoCloseRoutine = StartCoroutine(AutoClose());

        // Si vuelve a entrar el ratón, cancelamos el autocierre
        if ((overDrawer || overHandle) && autoCloseRoutine != null)
        {
            StopCoroutine(autoCloseRoutine);
            autoCloseRoutine = null;
        }
    }

    // -------------------------
    // TRANSITIONS
    // -------------------------
    void StartOpening()
    {
        StopRumble();
        state = DrawerState.Opening;
        UpdateArrowRotation();

        AudioController.Instance.PlaySFX(SFX.Pipe, (int)PipeSFX.DrawerOpening);

        drawer.transform.DOMove(openPosition.position, 1f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                state = DrawerState.Open;
            });
    }

    void StartClosing()
    {
        StopRumble();
        state = DrawerState.Closing;
        UpdateArrowRotation();

        AudioController.Instance.PlaySFX(SFX.Pipe, (int)PipeSFX.DrawerClosing);

        drawer.transform.DOMove(closedPosition.position, 1f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                state = DrawerState.Closed;
            });
    }

    IEnumerator AutoClose()
    {
        yield return new WaitForSecondsRealtime(3f);
        autoCloseRoutine = null;

        state = DrawerState.AutoClosing;
        UpdateArrowRotation();

        AudioController.Instance.PlaySFX(SFX.Pipe, (int)PipeSFX.DrawerClosing);

        drawer.transform.DOMove(closedPosition.position, 1f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                state = DrawerState.Closed;
            });
    }

    // -------------------------
    // RUMBLE
    // -------------------------
    void StartRumble()
    {
        if (rumbleTween != null && rumbleTween.IsActive()) return;

        rumbleTween = drawer.transform.DOLocalMoveX(
            drawer.transform.localPosition.x + 0.05f,
            0.4f
        )
        .SetEase(Ease.InOutSine)
        .SetLoops(-1, LoopType.Yoyo)
        .OnKill(() =>
        {
            drawer.transform.localPosition = closedPosition.localPosition;
        });
    }

    void StopRumble()
    {
        if (rumbleTween != null)
        {
            rumbleTween.Kill();
            rumbleTween = null;
        }
    }

    // -------------------------
    // RAYCASTS
    // -------------------------
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

    void UpdateArrowRotation()
    {
        if (state == DrawerState.Closed || state == DrawerState.Closing)
            arrow.rectTransform.localRotation = Quaternion.Euler(0, 0, 270);
        else
            arrow.rectTransform.localRotation = Quaternion.Euler(0, 0, 90);
    }

    void UpdateArrowColor(bool overHandle)
    {
        if (state == DrawerState.Opening || state == DrawerState.Closing || state == DrawerState.AutoClosing)
        {
            arrow.color = disabledColor;
            return;
        }

        if (Input.GetMouseButton(0) && overHandle)
            arrow.color = pressedColor;
        else if (overHandle)
            arrow.color = hoverColor;
        else
            arrow.color = normalColor;
    }
}