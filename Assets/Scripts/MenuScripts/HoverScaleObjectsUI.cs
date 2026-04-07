using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverScaleObjectsUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Vector3 gOHover;

	private void Start()
	{
		gOHover = transform.localScale;
	}

	public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(gOHover * 1.5f, 0.5f);

        AudioController.Instance.PlaySFX(SFX.Menu, (int)MenuSFX.Hover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
		transform.DOScale(gOHover, 0.5f);
	}
}
