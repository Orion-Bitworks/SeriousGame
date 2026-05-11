using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectOnEnable : MonoBehaviour
{
    [SerializeField] Button defaultButton;
    [SerializeField] bool scale = true;

    Vector3 gOHover;

    private void Awake()
    {
        Debug.Log("Awake en: " + name);
        gOHover = defaultButton.transform.localScale;
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable en: " + name);
        StartCoroutine(SelectNextFrame());
    }

    IEnumerator SelectNextFrame()
    {
        yield return null;

        if (defaultButton != null)
        {
            defaultButton.interactable = true;
            defaultButton.transform.localScale = gOHover;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);

            if (scale)
            {
                defaultButton.transform.DOScale(gOHover * 1.5f, 0.5f).SetUpdate(true);
            }
            
            Debug.Log("Selected: " + EventSystem.current.currentSelectedGameObject);
        }
    }
}
