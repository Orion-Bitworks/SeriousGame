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
				DialogManager.instance.Show("dialog_9_isbad");
				allRight = false;
			}
		}

		if (connections.Count == 0)
		{
			allRight = false;
		}

		if (allRight)
		{
			DialogManager.instance.Show("dialog_8_isgood");

			checkTV.ChangeState(CHECKING_STATE.CORRECT);
			DOVirtual.DelayedCall(1f, () =>
			{
				ToggleWidget(true);
			});
		}
		else
		{
			checkTV.ChangeState(CHECKING_STATE.WRONG);
			ResetLevel();
		}
	}

	private void ToggleWidget(bool state)
	{
		if (DialogManager.IsDialogActive)
		{
			DialogManager.pendingEvents.Enqueue(() => ToggleWidget(state));
			return;
		}

		widget.SetActive(state);
	}

	public void End3DMinigame()
	{
		DialogManager.instance.Show("dialog_10");

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

		if (!canPlay)
		{
			return;
		}

		playing = true;

		checkTV.ShowTV();
		checkTV.ChangeState(CHECKING_STATE.LOADING);

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

		checkTV.HideTV();

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

		checkTV.HideTV();

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
}