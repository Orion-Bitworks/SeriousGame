using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowInfoInCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI namePiecesText;
    [SerializeField] TextMeshProUGUI numberOfPiecesCollocatedText;

    private void Update()
    {
        ShowInfoNames();
        ShowInfoPieces();
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

    void ShowInfoPieces()
    {

    }
}
