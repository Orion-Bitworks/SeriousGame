using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICounter : MonoBehaviour
{
    [SerializeField] MolCountSO molCountSO;
	[SerializeField] TextMeshProUGUI totalO2Text;

	[SerializeField] TextMeshProUGUI totalAllFinishText;
	[SerializeField] TextMeshProUGUI totalO2FinishText;
	[SerializeField] TextMeshProUGUI totalCO2FinishText;

	private void OnEnable()
	{
		molCountSO.OnValueChanged += UpdateUI;
		UpdateUI();
	}

	private void OnDisable()
	{
		molCountSO.OnValueChanged -= UpdateUI;
	}
	public void UpdateUI()
	{
		totalO2Text.text = molCountSO.totalO2.ToString();
		totalAllFinishText.text = molCountSO.totalO2.ToString();
		totalO2FinishText.text = molCountSO.O2.ToString();
		totalCO2FinishText.text = molCountSO.CO2.ToString();

	}
}
