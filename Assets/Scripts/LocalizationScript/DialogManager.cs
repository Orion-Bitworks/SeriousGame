using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.AddressableAssets.Build.Layout.BuildLayout;

public class DialogManager : MonoBehaviour
{
    public static DialogManager instance;

    [SerializeField] private DialogBubbleController bubbleController;
	[SerializeField] public GameObject grid;
	public static Queue<System.Action> pendingEvents = new Queue<System.Action>();

	public static bool IsDialogActive = false;

	private void Awake()
	{
		instance = this;
		bubbleController.gameObject.SetActive(false);
	}

	public void Show(string key)
	{
		if (IsDialogActive)
		{
			pendingEvents.Enqueue(() => Show(key));
			return;
		}
		StartCoroutine(ShowAndWait(key));
	}

	public IEnumerator ShowAndWait(string key)
	{
		grid.SetActive(false);
		IsDialogActive = true;
		bubbleController.gameObject.SetActive(true);
		bubbleController.SetKey(key);


		yield return new WaitUntil(() => bubbleController.WasContinuePressed());

		bubbleController.gameObject.SetActive(false);

		IsDialogActive = false;
		grid.SetActive(true);

		// Ejecutar eventos pendientes
		if (pendingEvents.Count > 0)
		{
			var nextEvent = pendingEvents.Dequeue();
			nextEvent.Invoke();
		}
	}

	public void Hide()
	{

		bubbleController.gameObject.SetActive(false);
	}

	public void ShowSequence(string[] keys)
	{
		if (IsDialogActive)
		{
			pendingEvents.Enqueue(() => ShowSequence(keys));
			return;
		}
		StartCoroutine(SequenceRoutine(keys));
	}

	IEnumerator SequenceRoutine(string[] keys)
	{
		IsDialogActive = true;

		bubbleController.gameObject.SetActive(true);


		foreach (string key in keys)
		{
			Debug.Log("Mostrando frase: " + key);
			bubbleController.SetKey(key);
			yield return new WaitUntil(() => bubbleController.WasContinuePressed());

		}
		bubbleController.gameObject.SetActive(false);

		IsDialogActive = false;

		if (pendingEvents.Count > 0)
		{
			var nextEvent = pendingEvents.Dequeue();
			nextEvent.Invoke();
		}
	}
}
