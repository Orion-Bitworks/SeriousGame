using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnVein : MonoBehaviour
{
	[SerializeField] public MolCountSO countmMol;
	[SerializeField] PoolMoleculas poolMoleculas;

	private void Awake()
	{
		if(poolMoleculas == null)
		{
			poolMoleculas = FindAnyObjectByType<PoolMoleculas>();
		}

	}
	private void OnTriggerEnter(Collider other)
	{
		if (!other.CompareTag("MolAlv") && !other.CompareTag("MolVein"))
		{
			Debug.Log("no contamos");
			return;
		}

		if (other.CompareTag("MolAlv"))
		{
			countmMol.AddO2();
			Debug.Log("zñadimos o2");

		}
		else
		{
			countmMol.AddCO2();
			Debug.Log("añadimos co2");

		}
		poolMoleculas.Return(other.gameObject);
		countmMol.GetParameters();

	}
}
