using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolMoleculas : MonoBehaviour
{
    [SerializeField] GameObject molPrefab;
    [SerializeField] int initialSize = 15;

    readonly List<GameObject> pool = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewObject();
        }
    }

    GameObject CreateNewObject()
    {
        var obj = Instantiate(molPrefab);
        obj.SetActive(false);
        pool.Add(obj);
        return obj;
    }

    public GameObject Get()
    {
        foreach (var obj in pool)
        {
            if(!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        return CreateNewObject();
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
    }
}
