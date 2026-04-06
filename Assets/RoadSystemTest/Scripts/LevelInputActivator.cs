using UnityEngine;

public class LevelInputActivator : MonoBehaviour
{
    [Header("Inputs a activar")]
    [SerializeField] RoadInput[] inputsToActivate;

    [Header("Outputs a vigilar")]
    [SerializeField] RoadOutput[] outputsToCheck;

    [SerializeField] bool endingCondition = false;

    bool activated = false;
    bool IsGameFinished;

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
            GameManager.Instance.EndLevel();
            IsGameFinished = true;
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