using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [SerializeField] GameObject widget;

    [SerializeField] public HashSet<ConnectionPointController> connections = new HashSet<ConnectionPointController>();

    private bool resetting = false;

    private void Awake()
    {
        instance = this;
    }

    public void RegisterConnectionPoint(ConnectionPointController point)
    {
        connections.Add(point);
    }

    public void UnregisterConnectionPoint(ConnectionPointController point)
    {
        connections.Remove(point);
    }

    public void CheckConnections()
    {
        if (resetting)
        {
            return;
        }

        bool allRight = true;

        foreach (ConnectionPointController point in connections)
        {
            if (!point.PairedWithPartner())
            {
                Debug.Log("Incorrecto");
                allRight = false;
            }
        }

        if (connections.Count == 0)
        {
            allRight = false;
        }

        if (allRight)
        {
            ToggleWidget(true);
        }
        else
        {
            ResetLevel();
        }
    }

    private void ToggleWidget(bool state)
    {
        widget.SetActive(state);
    }

    public void End3DMinigame()
    {
        FindObjectOfType<GameLoopController>().End3DLevel();
    }

    public void LoadScene(string targetScene)
    {
        SceneManager.LoadScene(targetScene);
    }

    public void PlayFinishAnimation()
    {
        foreach (EventClick eventClick in FindObjectsOfType<EventClick>())
        {
            eventClick.CanInteract(false);
        }

        foreach (ConnectionPointController point in connections)
        {
            point.GetPiece().GetGroup().PlayFinishAnimation();
        }

        StartCoroutine(ShowPlayState());
    }

    public void ResetLevel()
    {
        resetting = true;

        ToggleWidget(false);

        StartCoroutine(StopBloodFlow());
    }

    public IEnumerator StopBloodFlow()
    {
        foreach (AnimatedPipeController pipe in FindObjectsOfType<AnimatedPipeController>())
        {
            pipe.StopSpawning();
        }

        yield return new WaitForSeconds(2f);

        foreach (ConnectionPointController point in connections)
        {
            point.GetPiece().GetGroup().RetrievePipesAnimation();
        }

        StartCoroutine(DeleteAllPieces());
    }

    public IEnumerator DeleteAllPieces()
    {
        ParticleManager.instance.StopAllParticles();

        yield return new WaitUntil(() => FindObjectsOfType<AnimatedPipeController>().Length == 0);

        ParticleManager.instance.DeleteAllParticles();

        Sequence sequence = DOTween.Sequence().SetAutoKill(true);

        foreach (ConnectionPointController point in connections)
        {
            point.Disable();
            sequence.Join(point.GetPiece().transform.DOMoveY(-5f, 0.5f).SetEase(Ease.InBack, 0.5f));
        }

        sequence.OnComplete(() =>
        {
            while (connections.Count > 0)
            {
                ConnectionPointController point = connections.First();
                point.GetPiece().DeletePiece();
            }

            resetting = false;
        });

        foreach (ConnectionPointController point in connections)
        {
            point.Enable();
        }

        foreach (BloodAnimationController controller in FindObjectsOfType<BloodAnimationController>())
        {
            controller.AlreadyFlowing(false);
        }

        foreach (EventClick eventClick in FindObjectsOfType<EventClick>())
        {
            eventClick.CanInteract(true);
        }
    }

    public IEnumerator ShowPlayState()
    {
        float timeDelay = 4f;

        timeDelay += connections.First().GetPiece().GetGroup().GetPieces().Count();

        yield return new WaitForSeconds(4f);

        if (FindObjectOfType<AnimatedPipeController>() == null)
        {
            timeDelay = 0f;
        }

        yield return new WaitForSeconds(timeDelay);
        CheckConnections();
    }
}
