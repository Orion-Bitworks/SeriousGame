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
    [SerializeField] CheckTVController checkTV;

    [SerializeField] public HashSet<ConnectionPointController> connections = new HashSet<ConnectionPointController>();
    [SerializeField] public HashSet<GameObject> pieces = new HashSet<GameObject>();

    private bool resetting = false;
    private bool playing = false;

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
            ChangeTVState(CHECKING_STATE.CORRECT);
            DOVirtual.DelayedCall(1f, () =>
            {
                ToggleWidget(true);
            });
        }
        else
        {
            ChangeTVState(CHECKING_STATE.WRONG);
            ResetLevel();
        }
    }

    private void ToggleWidget(bool state)
    {
        widget.SetActive(state);
    }

    public void End3DMinigame()
    {
        ResetLevel(true);
    }

    public void LoadScene(string targetScene)
    {
        SceneManager.LoadScene(targetScene);
    }

    public void PlayFinishAnimation()
    {
        if (playing)
        {
            return;
        }

        bool canPlay = false;

        foreach (EventClick eventClick in FindObjectsOfType<EventClick>())
        {
            if (!eventClick.OnUI())
            {
                canPlay = true;
            }
        }

        if (!canPlay || PieceGroupManager.GetGroupCount() > 1)
        {
            Debug.Log("Can't play because there are " + PieceGroupManager.GetGroupCount() + " active groups on scene.");
            return;
        }

        playing = true;

        ShowTV(true);
        ChangeTVState(CHECKING_STATE.LOADING);
        
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

    public void ResetLevel(bool continueToScene = false)
    {
        resetting = true;

        ToggleWidget(false);

        StartCoroutine(StopBloodFlow(continueToScene));
    }

    public IEnumerator StopBloodFlow(bool continueToScene = false)
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

        if (!continueToScene)
        {
            StartCoroutine(DeleteAllPieces());
        }
        else
        {
            StartCoroutine(ChangeScene());
        }
    }

    public IEnumerator ChangeScene()
    {
        yield return new WaitUntil(() => FindObjectsOfType<AnimatedPipeController>().Length == 0);

        ShowTV(false);

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

        yield return new WaitForSeconds(0.5f);

        FindObjectOfType<GameLoopController>().End3DLevel();
        /*foreach (GameObject piece in pieces)
        {
            Destroy(piece);
        }*/
    }

    public IEnumerator DeleteAllPieces()
    {
        ParticleManager.instance.StopAllParticles();

        yield return new WaitUntil(() => FindObjectsOfType<AnimatedPipeController>().Length == 0);

        ParticleManager.instance.DeleteAllParticles();

        ShowTV(false);

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

            foreach (ConnectionPointController point in connections)
            {
                point.Enable();
            }
        });

        foreach (BloodAnimationController controller in FindObjectsOfType<BloodAnimationController>())
        {
            controller.AlreadyFlowing(false);
        }

        foreach (EventClick eventClick in FindObjectsOfType<EventClick>())
        {
            eventClick.CanInteract(true);
        }

        playing = false;
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

    public void ChangeTVState(CHECKING_STATE newState)
    {
        if (checkTV != null)
        {
            checkTV.ChangeState(newState);
        }
    }

    public void ShowTV(bool b)
    {
        if (checkTV != null)
        {
            if (b)
            {
                checkTV.ShowTV();
            }
            else
            {
                checkTV.HideTV();
            }
        }
    }
}
