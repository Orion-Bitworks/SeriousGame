using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenuController : MonoBehaviour
{

    [SerializeField] GameObject[] allOptionPanels;
    GameObject currentPanel;
    [SerializeField] GameObject currentRebindPanel;

    private void Start()
    {

        foreach(GameObject panel in allOptionPanels) {
            
            if (panel.name == "SoundPanel")
            {
                panel.SetActive(true);
                currentPanel = panel;
                currentRebindPanel.SetActive(true);
            }
            else
            {
                panel.SetActive(false);
            }
                 
        }

    }

    public void ChangeMenu(GameObject activePanel)
    {
        if (currentPanel == activePanel) return;

        currentPanel.SetActive(false); 
        currentPanel = activePanel;
        currentPanel.SetActive(true);

    }

    public void ChangeRebindMenu(GameObject activeRebindPanel)
    {

        currentRebindPanel.SetActive(false);
        currentRebindPanel = activeRebindPanel;
        currentRebindPanel.SetActive(true);
        
    }

}
