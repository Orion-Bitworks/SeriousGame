using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RandomDeactivation : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsToDisable = new List<GameObject>();

    [Header("Timing")]
    [SerializeField] private float minTimeBetweenFlickers = 2f;
    [SerializeField] private float maxTimeBetweenFlickers = 10f;

    [Header("Flicker")]
    [SerializeField] private int minFlickers = 2;
    [SerializeField] private int maxFlickers = 8;

    [SerializeField] private float minInactiveTime = 0.03f;
    [SerializeField] private float maxInactiveTime = 0.15f;

    [SerializeField] private float minActiveTime = 0.02f;
    [SerializeField] private float maxActiveTime = 0.08f;

    [Header("Stop Working")]
    [SerializeField] private bool canStopWorking = false;
    [SerializeField] private bool working = true;
    [SerializeField] private Transform particlePivot;

    private Vector3 particlePos;

    private void Start()
    {
        if (particlePivot != null)
        {
            particlePos = particlePivot.position;
        }
        else
        {
            particlePos = transform.position;
        }

        StartCoroutine(FlickerLoop());
    }

    private IEnumerator FlickerLoop()
    {
        while (working)
        {
            float timer = Random.Range(minTimeBetweenFlickers, maxTimeBetweenFlickers);
            yield return new WaitForSeconds(timer);

            int flickerCount = Random.Range(minFlickers, maxFlickers + 1);

            for (int i = 0; i < flickerCount; i++)
            {
                SetState(false);

                //ParticleManager.instance.SpawnParticles("Flash", transform.position, Quaternion.identity);

                yield return new WaitForSeconds(Random.Range(minInactiveTime, maxInactiveTime));

                SetState(true);

                yield return new WaitForSeconds(Random.Range(minActiveTime, maxActiveTime));
            }

            if (Random.value < 0.25f)
            {
                SetState(false);

                yield return new WaitForSeconds(Random.Range(0.5f, 2f));

                SetState(true);
            }

            if (canStopWorking && Random.value < 0.4f)
            {
                ParticleManager.instance.SpawnParticles("Flash", particlePos, Quaternion.identity);
                working = false;
                SetState(false);
            }
        }
    }

    private void SetState(bool state)
    {
        foreach (GameObject go in objectsToDisable)
        {
            go.SetActive(state);
        }
    }
}