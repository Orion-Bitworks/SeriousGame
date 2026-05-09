using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;


public class ShowInfoInCanvas : MonoBehaviour
{
    [SerializeField] LocalizeStringEvent localizedText;

    private void Update()
    {
        if (ObjectSelector.currentlySelected != null)
        {
            localizedText.StringReference.TableEntryReference = ObjectSelector.currentlySelected.name;
            localizedText.RefreshString();
        }
        else
        {
            localizedText.StringReference.TableEntryReference = "";
            localizedText.RefreshString();
        }
    }
}