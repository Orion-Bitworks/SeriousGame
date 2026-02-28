using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [SerializeField] Transform canvas;
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

    public void CheckConnections()
    {
        bool allRight = true;

        foreach (ConnectionPointController point in connections)
        {
            if (!point.PairedWithPartner())
            {
                allRight = false;
            }
        }

        if (allRight)
        {
            ToggleWidget();
        }
    }

    private void ToggleWidget()
    {
        Instantiate(widget, canvas);
    }
}
