using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GameLoopController : MonoBehaviour
{
    [SerializeField] GameObject heartMinigames;
    [SerializeField] GameObject heartMinigamesObject;
    [SerializeField] GameObject heartMinigamesUI;
    [SerializeField] GameObject threeDMinigame;
    [SerializeField] GameObject threeDMinigamePieces;
    [SerializeField] GameObject threeDMinigameScreen;
    [SerializeField] CinemachineVirtualCamera pipesCamera;
    [SerializeField] CinemachineVirtualCamera heartMinigamesCamera;
    [SerializeField] CinemachineVirtualCamera threeDMinigameCamera;
    [SerializeField] TutorialManager controller;
    [SerializeField] ScreenController screen;
    [SerializeField] GameObject stopPanel;
    [SerializeField] GameObject virtualMouseCanvas;

    bool minigamesStarted = true;
    bool minigamesFinished = false;
    bool threeDStarted = true;
    bool threeDFinished = false;

    OrganLogic heartObject;

    private void Start()
    {
        GameManager.Instance.OnOrganPlaced += HandleHeartPlaced;
        FindObjectOfType<NewMinigame3>(true).OnGameCompleted += HandleGameCompleted;
    }

    private void HandleHeartPlaced(OrganData organ, Vector3 organPosition)
    {
        if (organ.organType != OrganType.Heart) return;

        if (minigamesStarted) return;

        stopPanel.SetActive(false);

        StartCoroutine(DelayedHandleHeartPlaced(organPosition));
    }

    IEnumerator DelayedHandleHeartPlaced(Vector3 organPosition)
    {
        yield return new WaitForNextFrameUnit();

        minigamesStarted = true;
        GameManager.Instance.isPlaying = true;
        BuildController.Instance.controls.Disable();
        GameManager.Instance.currentLevelGameObject.SetActive(false);
        heartMinigames.gameObject.SetActive(true);
        heartMinigames.transform.position = organPosition;

        yield return new WaitForSecondsRealtime(1f);

        heartObject = FindAnyObjectByType<OrganLogic>();
        heartObject.gameObject.SetActive(false);
        heartMinigamesObject.SetActive(true);
        heartMinigamesUI.gameObject.SetActive(true);
        virtualMouseCanvas.gameObject.SetActive(true);
        heartMinigamesCamera.Priority = 2;
        DialogManager.instance.Show("dialog_14");
	}

    IEnumerator DelayedHandleGameCompleted()
    {
        minigamesFinished = true;
        GameManager.Instance.currentLevelGameObject.SetActive(true);
        GameManager.Instance.isPlaying = false;
        BuildController.Instance.controls.Enable();

        screen.AppearScreenOff();

        yield return new WaitForSecondsRealtime(1f);

        heartMinigamesObject.SetActive(false);
        heartObject.gameObject.SetActive(true);
        heartMinigamesUI.gameObject.SetActive(false);
        virtualMouseCanvas.gameObject.SetActive(false);
        heartMinigamesCamera.Priority = 0;
        
        yield return new WaitForSecondsRealtime(1f);

        screen.StartMovingOut();

        yield return new WaitForSecondsRealtime(2f);

        heartMinigames.gameObject.SetActive(false);
    }

    private void HandleGameCompleted(bool completed)
    {
        if (!completed || minigamesFinished) return;

        DialogManager.instance.Show("dialog_25");
        controller.ShowTutorial(5);
        StartCoroutine(DelayedHandleGameCompleted());
    }

	private IEnumerator DelayedStart3DLevel()
    {
        yield return new WaitForEndOfFrame();

        if (threeDStarted) yield break;

        threeDStarted = true;
        GameManager.Instance.isPlaying = true;
        BuildController.Instance.controls.Disable();
        GameManager.Instance.currentLevelGameObject.SetActive(false);
        threeDMinigame.gameObject.SetActive(true);
        threeDMinigamePieces.gameObject.SetActive(true);
        threeDMinigameScreen.gameObject.SetActive(true);
        virtualMouseCanvas.gameObject.SetActive(true);
        threeDMinigameCamera.Priority = 2;
		DialogManager.instance.Show("dialog_7");
        controller.ShowTutorial(1);
	}

    public void Start3DLevel()
    {
        StartCoroutine(DelayedStart3DLevel());
    }

    public void End3DLevel()
    {
        if (threeDFinished) return;

        threeDFinished = true;
        GameManager.Instance.currentLevelGameObject.SetActive(true);
        GameManager.Instance.isPlaying = false;
        BuildController.Instance.controls.Enable();
        threeDMinigame.gameObject.SetActive(false);
        threeDMinigamePieces.gameObject.SetActive(false);
        threeDMinigameScreen.gameObject.SetActive(false);
        virtualMouseCanvas.gameObject.SetActive(false);
        threeDMinigameCamera.Priority = 0;
        DialogManager.instance.ShowSequence(new string []{ "dialog_11", "dialog_12", "dialog_13" });
        DialogManager.pendingEvents.Enqueue(() => BuildController.Instance.controls.Disable());
        stopPanel.SetActive(true);
    }
}