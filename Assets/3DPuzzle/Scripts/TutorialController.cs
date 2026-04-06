using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private string tagToDetect;
	[SerializeField] private TutorialLocalizationController localizationController;
	[SerializeField] GameObject carpeta;


	[Header("Navigation Buttons")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button priorButton;

    [Header("Progress Dots")]
    [SerializeField] private Transform progressPanel;
    [SerializeField] private Sprite progressSprite;
    [SerializeField] private Color unselected = new Color(0.56f, 0.56f, 0.56f);
    [SerializeField] private Color selected = new Color(1f, 1f, 1f);

    private List<GameObject> panels = new List<GameObject>();
    private List<GameObject> dots = new List<GameObject>();
    private GameObject activePanel;
    private int panelIndex = 0;
	public int tutorialID = 5;

	private void Awake()
    {
        RegisterPanels();
    }

    private void Start()
    {
        GenerateProgressCircles();
        panelIndex = 0;
        ChangePanel();
	}

    private void OnEnable()
    {
        ResetTutorial();
    }

    public void ChangePanel() {
        if (activePanel)
        {
            activePanel.gameObject.SetActive(false);
        }
        
        panels[panelIndex].SetActive(true);
        activePanel = panels[panelIndex];

        localizationController.SetPage(panelIndex);

        ToggleButtons();
        SelectProgressDot();
    }

    public void NextPanel()
    {
		panelIndex++;
		panelIndex = Mathf.Clamp(panelIndex, 0, panels.Count - 1);

		ChangePanel();
	}

    public void PriorPanel()
    {
		panelIndex--;
		panelIndex = Mathf.Clamp(panelIndex, 0, panels.Count - 1);

		ChangePanel();
	}

    public void ResetTutorial()
    {
        panelIndex = 0;
        ChangePanel();
    }

    public void CloseTutorial()
    {
        carpeta.SetActive(false);
        gameObject.SetActive(false);
    }

    public void OpenTutorial()
    {
        gameObject.SetActive(true);
    }

    private void RegisterPanels()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == tagToDetect)
            {
                panels.Add(child.gameObject);
            }

		}
    }

    private void GenerateProgressCircles()
    {
        foreach (GameObject panel in panels)
        {
            GameObject progressDot = new GameObject();
            progressDot.layer = 5; // Layer: UI
            progressDot.name = "Progress Dot";

            Image circleImage = progressDot.AddComponent<Image>();
            circleImage.sprite = progressSprite;
            circleImage.color = unselected;

            Outline outline = progressDot.AddComponent<Outline>();
            outline.effectColor = unselected;

            progressDot.transform.SetParent(progressPanel, false);

            dots.Add(progressDot);
        }
    }

    private void ToggleButtons()
    {
        if (panelIndex == panels.Count -1)
        {
            nextButton.gameObject.SetActive(false);
        }
        else
        {
            nextButton.gameObject.SetActive(true);
        }

        if (panelIndex == 0)
        {
            priorButton.gameObject.SetActive(false);
        }
        else
        {
            priorButton.gameObject.SetActive(true);
        }
    }

    private void SelectProgressDot()
    {
        for (int i = 0; i < dots.Count; i++)
        {
            if (i != panelIndex)
            {
                dots[i].GetComponent<Image>().color = unselected;
                dots[i].GetComponent<Outline>().effectColor = new Color(0, 0, 0, 0);
                dots[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
            else
            {
                dots[i].GetComponent<Image>().color = selected;
                dots[i].GetComponent<Outline>().effectColor = unselected;
                dots[i].transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }
}
