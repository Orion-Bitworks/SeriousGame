using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class WorldSpaceButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public Renderer targetRenderer;
    public Color normalColor = Color.white;
    public Color hoverColor = new Color (0.7f, 0.7f, 0.7f, 1);
    public Color pressedColor = Color.gray;
    enum Action { rotate, right, left };
    [SerializeField] Action action;

    void Start()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        targetRenderer.material.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetRenderer.material.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetRenderer.material.color = normalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetRenderer.material.color = pressedColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetRenderer.material.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }

    public void OnClick()
    {
        switch (action)
        {
            case Action.rotate:
                BuildController.Instance.RotateGhostAndPreview();
                break;
            case Action.right:
                BuildController.Instance.ChangeObject(true);
                break;
            case Action.left:
                BuildController.Instance.ChangeObject(false);
                break;
        }
    }
}