using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [SerializeField] GameObject widget;

    [SerializeField] public HashSet<ConnectionPointController> connections = new HashSet<ConnectionPointController>();
    [SerializeField] public HashSet<GameObject> pieces = new HashSet<GameObject>();

    private void Awake()
    {
        instance = this;
    }

    public void RegisterConnectionPoint(ConnectionPointController point)
    {
        connections.Add(point);
    }

    public void UnregisterConnectionPoint(ConnectionPointController point)
    {
        connections.Remove(point);
    }

    public void CheckConnections()
    {
        bool allRight = true;

        foreach (ConnectionPointController point in connections)
        {
            if (!point.PairedWithPartner())
            {
                DialogManager.instance.Show("dialog_9_isbad");

                Debug.Log("Incorrecto");
                allRight = false;
            }
        }

        if (connections.Count == 0)
        {
            allRight = false;
        }

        if (allRight)
        {
            DialogManager.instance.Show("dialog_8_isgood");
            ToggleWidget();
            
        }
    }


    private void ToggleWidget()
    {
		if (DialogManager.IsDialogActive)
		{
			DialogManager.pendingEvents.Enqueue(() => ToggleWidget());
			return;
		}

		widget.SetActive(true);
	}

    public void End3DMinigame()
    {
        DialogManager.instance.Show("dialog_10");
        FindObjectOfType<GameLoopController>().End3DLevel();
        foreach (GameObject piece in pieces)
        {
            Destroy(piece);
        }
    }

    public void LoadScene(string targetScene)
    {
        SceneManager.LoadScene(targetScene);
    }
}
