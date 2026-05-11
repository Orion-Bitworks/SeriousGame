using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectOnEnable : MonoBehaviour
{
    [SerializeField] Button defaultButton;

    Vector3 gOHover;

    private void Awake()
    {
        gOHover = defaultButton.transform.localScale;
    }

    private void OnEnable()
    {
        if (defaultButton != null)
        {
            defaultButton.transform.localScale = gOHover;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
            defaultButton.transform.DOScale(gOHover * 1.5f, 0.5f).SetUpdate(true);
        }
    }
}
