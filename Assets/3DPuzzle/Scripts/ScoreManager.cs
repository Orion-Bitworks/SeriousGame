using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [SerializeField] GameObject widget;

    [SerializeField] public HashSet<ConnectionPointController> connections = new HashSet<ConnectionPointController>();

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
            ToggleWidget();
        }
    }

    private void ToggleWidget()
    {
        widget.SetActive(true);
    }

    public void End3DMinigame()
    {
        FindObjectOfType<GameLoopController>().End3DLevel();
    }

    public void LoadScene(string targetScene)
    {
        SceneManager.LoadScene(targetScene);
    }
}
