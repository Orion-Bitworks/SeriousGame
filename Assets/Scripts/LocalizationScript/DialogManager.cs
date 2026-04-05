using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.AddressableAssets.Build.Layout.BuildLayout;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;

    [SerializeField] private DialogBubbleController bubbleController;

	private void Awake()
	{
		instance = this;
		bubbleController.gameObject.SetActive(false);
	}

	public void Show(string key)
	{
		bubbleController.gameObject.SetActive(true);
		bubbleController.SetKey(key);
		StartCoroutine(ShowAndWait(key));
	}

	public IEnumerator ShowAndWait(string key)
	{
		yield return new WaitUntil(() => bubbleController.WasContinuePressed());

		bubbleController.gameObject.SetActive(false);
	}

	public void Hide()
	{

		bubbleController.gameObject.SetActive(false);
	}

	public void ShowSequence(string[] keys)
	{
		StartCoroutine(SequenceRoutine(keys));
	}

	IEnumerator SequenceRoutine(string[] keys)
	{
		bubbleController.gameObject.SetActive(true);

		foreach (string key in keys)
		{
			Debug.Log("Mostrando frase: " + key);
			bubbleController.SetKey(key);
			yield return new WaitUntil(() => bubbleController.WasContinuePressed());

		}
		bubbleController.gameObject.SetActive(false);
	}
}
