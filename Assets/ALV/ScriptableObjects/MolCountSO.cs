using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName =("Moleculas/Contador"))]
public class MolCountSO : ScriptableObject
{
	public int O2;
	public int CO2;
	public int totalO2;

	public event Action OnValueChanged;

	public void AddO2()
	{
		O2++;
		Total();
	}
	public void AddCO2()
	{
		CO2++;
		Total();
	}

	public void Total()
	{
		totalO2 = O2 - CO2;
		OnValueChanged?.Invoke();

	}
	public void GetParameters()
	{
		Debug.Log("O2: " + O2);
		Debug.Log("CO2: " +  CO2);
	}

	public void RestoreParameters()
	{
		O2 = 0;
		CO2 = 0;
		totalO2 = 0;
		OnValueChanged.Invoke();
	}
}
