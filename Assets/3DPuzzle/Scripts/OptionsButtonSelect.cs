using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsButtonSelect : MonoBehaviour, ISelectHandler
{
    [SerializeField] ButtonsUIManagerController buttonManager;

    public void OnSelect(BaseEventData eventData)
    {
        Button button = GetComponent<Button>();

        if (button != null && buttonManager != null)
        {
            buttonManager.SelectButton(button);
        }
    }
}
