using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDeactivation : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsToDisable = new List<GameObject>();
    private bool active = true;

    private void Update()
    {
        float randomNum = Random.Range(0, 500f);

        if (randomNum < 1f)
        {
            ChangeState();
        }
    }

    private void ChangeState()
    {
        if (active)
        {
            active = false;

            foreach (GameObject go in objectsToDisable)
            {
                go.SetActive(false);
            }

            StartCoroutine(SetInactive());
        }
    }

    private IEnumerator SetInactive()
    {
        // SONIDO DE LUCES APAGANDOSE
        ParticleManager.instance.SpawnParticles("Flash", transform.position, Quaternion.identity);
        float timer = Random.Range(0.05f, 0.6f);

        yield return new WaitForSeconds(timer);

        foreach (GameObject go in objectsToDisable)
        {
            go.SetActive(true);
        }

        active = true;
    }
}
