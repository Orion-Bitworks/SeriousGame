using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPivotController : MonoBehaviour
{
    [SerializeField] private int priority = 1;

    [Header("Pipe Parameters")]

    [SerializeField] GameObject pipePrefab;
    [SerializeField] private float spawnDistance = 1;

    public int GetPriority()
    {
        return priority;
    }

    public void StartAnimation()
    {
        Vector3 spawnPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + spawnDistance);

        GameObject newPipe = Instantiate(pipePrefab, transform.position, transform.rotation);
        newPipe.transform.SetParent(transform, true);
        newPipe.transform.localPosition = spawnPosition;

    }
}
