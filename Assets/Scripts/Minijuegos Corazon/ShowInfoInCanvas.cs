using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowInfoInCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI namePiecesText;

    private void Update()
    {
        ShowInfoNames();
    }

    void ShowInfoNames()
    {
        if (ObjectSelector.currentlySelected != null)
        {
            namePiecesText.text = ObjectSelector.currentlySelected.name;
        }
        else
        {
            namePiecesText.text = " ";
        }
    }

}
