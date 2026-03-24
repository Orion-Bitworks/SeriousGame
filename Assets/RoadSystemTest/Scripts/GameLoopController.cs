using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GameLoopController : MonoBehaviour
{
    [SerializeField] GameObject heartMinigames;
    [SerializeField] GameObject heartMinigamesUI;
    [SerializeField] GameObject threeDMinigame;
    [SerializeField] GameObject threeDMinigameUI;
    [SerializeField] GameObject pipesUI;
    [SerializeField] CinemachineVirtualCamera pipesCamera;
    [SerializeField] CinemachineVirtualCamera heartMinigamesCamera;
    [SerializeField] CinemachineVirtualCamera threeDMinigameCamera;

    bool minigamesStarted = false;
    bool minigamesFinished = false;
    bool threeDStarted = false;
    bool threeDFinished = false;

    private void Start()
    {
        GameManager.Instance.OnHeartPlacedChanged += HandleHeartPlaced;
        FindObjectOfType<Minigame3>(true).OnGameCompleted += HandleGameCompleted;
    }

    private void HandleHeartPlaced(bool placed)
    {
        if (!placed || minigamesStarted) return;

        StartCoroutine(DelayedHandleHeartPlaced());
    }

    IEnumerator DelayedHandleHeartPlaced()
    {
        yield return new WaitForNextFrameUnit();

        minigamesStarted = true;
        GameManager.Instance.isPlaying = true;
        GameManager.Instance.currentLevelGameObject.SetActive(false);
        heartMinigames.gameObject.SetActive(true);
        heartMinigames.transform.position = GameManager.Instance.heartPosition;
        pipesUI.gameObject.SetActive(false);
        heartMinigamesUI.gameObject.SetActive(true);
        heartMinigamesCamera.Priority = 2;
    }

    private void HandleGameCompleted(bool completed)
    {
        if (!completed || minigamesFinished) return;

        minigamesFinished = true;
        GameManager.Instance.currentLevelGameObject.SetActive(true);
        GameManager.Instance.isPlaying = false;
        heartMinigames.gameObject.SetActive(false);
        pipesUI.gameObject.SetActive(true);
        heartMinigamesUI.gameObject.SetActive(false);
        heartMinigamesCamera.Priority = 0;
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
        threeDMinigameCamera.Priority = 0;
    }
}
