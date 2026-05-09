using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverScaleObjectsUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    Vector3 gOHover;

	private void Start()
	{
		gOHover = transform.localScale;
	}

    private void OnDisable()
    {
        ScaleDown();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(gOHover * 1.5f, 0.5f).SetUpdate(true);

        AudioController.Instance.PlaySFX(SFX.Menu, (int)MenuSFX.Hover);
    }

    private void ScaleDown()
    {
		transform.DOScale(gOHover, 0.5f).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ScaleDown();
	}

    public void OnSelect(BaseEventData eventData)
    {
        transform.DOScale(gOHover * 1.5f, 0.5f).SetUpdate(true);
        AudioController.Instance.PlaySFX(SFX.Menu, (int)MenuSFX.Hover);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        transform.DOScale(gOHover, 0.5f).SetUpdate(true);
    }
}
