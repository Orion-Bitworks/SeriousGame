using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenuController : MonoBehaviour
{
    [SerializeField] GameObject[] panel;
    GameObject currentPanel;
    //Button button;

    private void Start()
    {
        currentPanel = panel[0];
        panel[0].SetActive(true);
    }


    public void ChangeMenu(GameObject activePanel)
    {
        if (currentPanel == activePanel) return;

        activePanel.SetActive(true);
        currentPanel.SetActive(false);
        currentPanel = activePanel;
        
    }



}
