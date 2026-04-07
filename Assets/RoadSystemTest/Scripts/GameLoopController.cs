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
    [SerializeField] GameObject threeDMinigameUI;
    [SerializeField] GameObject threeDMinigamePieces;
    [SerializeField] GameObject pipesUI;
    [SerializeField] CinemachineVirtualCamera pipesCamera;
    [SerializeField] CinemachineVirtualCamera heartMinigamesCamera;
    [SerializeField] CinemachineVirtualCamera threeDMinigameCamera;

    bool minigamesStarted = false;
    bool minigamesFinished = false;
    bool threeDStarted = false;
    bool threeDFinished = false;

    OrganLogic heartObject;

    private void Start()
    {
        GameManager.Instance.OnOrganPlaced += HandleHeartPlaced;
        FindObjectOfType<Minigame3>(true).OnGameCompleted += HandleGameCompleted;
    }

    private void HandleHeartPlaced(OrganData organ, Vector3 organPosition)
    {
        if (organ.organType != OrganType.Heart) return;

        if (minigamesStarted) return;

        StartCoroutine(DelayedHandleHeartPlaced(organPosition));
    }

    IEnumerator DelayedHandleHeartPlaced(Vector3 organPosition)
    {
        yield return new WaitForNextFrameUnit();

        minigamesStarted = true;
        GameManager.Instance.isPlaying = true;
        GameManager.Instance.currentLevelGameObject.SetActive(false);
        heartMinigames.gameObject.SetActive(true);
        heartMinigames.transform.position = organPosition;

        yield return new WaitForSecondsRealtime(1f);

        heartObject = FindAnyObjectByType<OrganLogic>();
        heartObject.gameObject.SetActive(false);
        heartMinigamesObject.SetActive(true);
        pipesUI.gameObject.SetActive(false);
        heartMinigamesUI.gameObject.SetActive(true);
        heartMinigamesCamera.Priority = 2;
    }

    IEnumerator DelayedHandleGameCompleted()
    {
        minigamesFinished = true;
        GameManager.Instance.currentLevelGameObject.SetActive(true);
        GameManager.Instance.isPlaying = false;
        heartMinigamesObject.SetActive(false);
        heartObject.gameObject.SetActive(true);
        pipesUI.gameObject.SetActive(true);
        heartMinigamesUI.gameObject.SetActive(false);
        heartMinigamesCamera.Priority = 0;
        
        yield return new WaitForSecondsRealtime(1f);

        FindAnyObjectByType<ScreenController>().StartMovingOut();

        yield return new WaitForSecondsRealtime(2f);

        heartMinigames.gameObject.SetActive(false);
    }

    private void HandleGameCompleted(bool completed)
    {
        if (!completed || minigamesFinished) return;

        StartCoroutine(DelayedHandleGameCompleted());
    }

    private IEnumerator DelayedStart3DLevel()
    {
        yield return new WaitForSecondsRealtime(2f);

        if (threeDStarted) yield break;

        threeDStarted = true;
        GameManager.Instance.isPlaying = true;
        GameManager.Instance.currentLevelGameObject.SetActive(false);
        threeDMinigame.gameObject.SetActive(true);
        pipesUI.gameObject.SetActive(false);
        threeDMinigameUI.gameObject.SetActive(true);
        threeDMinigamePieces.gameObject.SetActive(true);
        threeDMinigameCamera.Priority = 2;
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
        threeDMinigame.gameObject.SetActive(false);
        pipesUI.gameObject.SetActive(true);
        threeDMinigameUI.gameObject.SetActive(false);
        threeDMinigamePieces.gameObject.SetActive(false);
        threeDMinigameCamera.Priority = 0;
    }
}
