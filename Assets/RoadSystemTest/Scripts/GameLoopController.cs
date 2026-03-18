using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GameLoopController : MonoBehaviour
{
    [SerializeField] GameObject heartMinigames;
    [SerializeField] GameObject heartMinigamesUI;
    [SerializeField] GameObject pipesUI;
    [SerializeField] CinemachineVirtualCamera pipesCamera;
    [SerializeField] CinemachineVirtualCamera heartMinigamesCamera;

    bool started = false;
    bool finished = false;

    private void Start()
    {
        GameManager.Instance.OnHeartPlacedChanged += HandleHeartPlaced;
        FindObjectOfType<Minigame3>(true).OnGameCompleted += HandleGameCompleted;
    }

    private void HandleHeartPlaced(bool placed)
    {
        if (!placed || started) return;

        StartCoroutine(WaitASecond());
    }

    IEnumerator WaitASecond()
    {
        yield return new WaitForNextFrameUnit();

        started = true;
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
        if (!completed || finished) return;

        finished = true;
        GameManager.Instance.isPlaying = false;
        GameManager.Instance.currentLevelGameObject.SetActive(true);
        heartMinigames.gameObject.SetActive(false);
        pipesUI.gameObject.SetActive(true);
        heartMinigamesUI.gameObject.SetActive(false);
        heartMinigamesCamera.Priority = 0;
    }
}
