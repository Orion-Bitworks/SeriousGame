using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [SerializeField] GameObject widget;

    [SerializeField] public HashSet<ConnectionPointController> connections = new HashSet<ConnectionPointController>();

    private bool resetting = false;

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
        if (resetting)
        {
            return;
        }

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
            ToggleWidget(true);
        }
    }

    private void ToggleWidget(bool state)
    {
        widget.SetActive(state);
    }

    public void End3DMinigame()
    {
        FindObjectOfType<GameLoopController>().End3DLevel();
    }

    public void LoadScene(string targetScene)
    {
        SceneManager.LoadScene(targetScene);
    }

    public void PlayFinishAnimation()
    {
        foreach (EventClick eventClick in FindObjectsOfType<EventClick>())
        {
            eventClick.CanInteract(false);
        }

        foreach (ConnectionPointController point in connections)
        {
            point.GetPiece().GetGroup().PlayFinishAnimation();
        }
    }

    public void ResetLevel()
    {
        resetting = true;

        ToggleWidget(false);

        foreach (ConnectionPointController point in connections)
        {
            point.GetPiece().GetGroup().RetrievePipesAnimation();
        }

        StartCoroutine(DeleteAllPieces());
    }

    public IEnumerator DeleteAllPieces()
    {
        yield return new WaitUntil(() => FindObjectsOfType<AnimatedPipeController>().Length == 0);

        while (connections.Count > 0)
        {
            ConnectionPointController point = connections.First();
            point.GetPiece().DeletePiece();
        }

        foreach (EventClick eventClick in FindObjectsOfType<EventClick>())
        {
            eventClick.CanInteract(true);
        }

        resetting = false;
    }
}
