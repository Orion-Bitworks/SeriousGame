using System.Collections;
using UnityEngine;

public class LevelInputActivator : MonoBehaviour
{
    [Header("Inputs a activar")]
    [SerializeField] RoadInput[] inputsToActivate;

    [Header("Outputs a vigilar")]
    [SerializeField] RoadOutput[] outputsToCheck;

    [SerializeField] bool endingCondition = false;

    RoadUIManager roadUIManager;

    bool activated = false;
    bool IsGameFinished;
    bool coroutineStarted = false;

    private void Awake()
    {
        roadUIManager = FindObjectOfType<RoadUIManager>();
    }

    private void Start()
    {
        // Aseguramos que todos los inputs empiezan apagados
        foreach (var input in inputsToActivate)
        {
            if (input != null)
                input.StopGenerating();
        }
		IsGameFinished = false;
	}

    private void Update()
    {
        if (!activated && CheckCondition())
        {
            ActivateInputs();
            activated = true;
        }

        if (endingCondition && CheckCondition() && !IsGameFinished)
        {
            if (!coroutineStarted)
                StartCoroutine(CheckWin());
        }
    }

    bool CheckCondition()
    {
        int count = 0;

        foreach (var output in outputsToCheck)
        {
            if (output != null && output.ballReceived)
                count++;
        }

        return count >= outputsToCheck.Length;
    }

    void ActivateInputs()
    {
        foreach (var input in inputsToActivate)
            input.StartGenerating();
    }

    IEnumerator CheckWin()
    {
        coroutineStarted = true;

        while (!AllPiecesFullyCovered() && !GameManager.Instance.failed)
            yield return null;

        yield return new WaitForSeconds(1f);

        if (!GameManager.Instance.failed)
        {
            GameManager.Instance.EndLevel();
            IsGameFinished = true;
            coroutineStarted = false;
        }
        else
        {
            roadUIManager.OnStopButtonDown();
            coroutineStarted = false;

            if (GameManager.Instance.currentLevel == LevelID.Pipe)
                DialogManager.instance.Show("dialog_5_idbad");
            else if (GameManager.Instance.currentLevel == LevelID.Heart)
                DialogManager.instance.Show("dialog_27_isbad");
        }
    }

    bool AllPiecesFullyCovered()
    {
        foreach (var kvp in GridManager.Instance.placedObjects)
        {
            RoadPiece piece = kvp.Value.GetComponent<RoadPiece>();
            if (piece == null) continue;

            if (!piece.wasUsed)
                continue;

            if (!PieceFullyCovered(piece))
                return false;
        }

        return true;
    }

    bool PieceFullyCovered(RoadPiece piece)
    {
        int used = 0;

        for (int i = 0; i < piece.exitUsed.Length; i++)
            if (piece.exitUsed[i])
                used++;

        return used >= piece.requiredExits;
    }

    public void DeactivateInputs()
    {
        activated = false;

        foreach (var input in inputsToActivate)
        {
            if (input != null)
                input.StopGenerating();
        }
    }
}