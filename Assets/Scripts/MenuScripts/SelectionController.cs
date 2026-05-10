using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionController : MonoBehaviour
{
    private Selectable lastSelected;

    private void OnEnable()
    {
        if (lastSelected != null)
        {
            lastSelected.Select();
        }
    }

    public void SetLastSelected(Selectable selection)
    {
        lastSelected = selection;
    }
}
