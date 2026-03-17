using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleculaLive : MonoBehaviour
{
    PoolMoleculas poolMoleculas;
    public float lifeTime = 5f;
    MoleculaObject mol;
    [SerializeField] public MolCountSO countmMol;


    private void Awake()
    {
        mol = GetComponent<MoleculaObject>();
        poolMoleculas = FindAnyObjectByType<PoolMoleculas>();


    }
    private void OnEnable()
    {
        if (mol.IsInVein())
        {
            return;
        }

        StartCoroutine(ReturnAfterTime());
    }
    
    IEnumerator ReturnAfterTime()
    {
        
        yield return new WaitForSeconds(lifeTime);
        poolMoleculas.Return(gameObject);
    }

	internal void OnZoneChanged(tipeZone tipo)
	{
        if (!gameObject.activeInHierarchy) return;

		StopAllCoroutines();

        if (tipo == tipeZone.Vena || tipo == tipeZone.None)
            return;

        StartCoroutine(ReturnAfterTime());
	}

}
