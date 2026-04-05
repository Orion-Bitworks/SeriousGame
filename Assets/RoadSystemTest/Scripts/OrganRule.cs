using System;
using UnityEngine;

[Serializable]
public class OrganRule
{
    public string ruleName;

    [Header("Condiciones (outputs que deben recibir bola)")]
    public RoadOutput[] requiredOutputs;

    [Header("Inputs que se activan cuando se cumple la regla")]
    public RoadInput[] inputsToActivate;

    [HideInInspector] public bool ruleTriggered = false;

    public bool CheckCondition()
    {
        foreach (var output in requiredOutputs)
        {
            if (!output.ballReceived)
                return false;
        }
        return true;
    }

    public void ExecuteRule()
    {
        if (ruleTriggered) return;

        foreach (var input in inputsToActivate)
            input.StartGenerating();

        ruleTriggered = true;
    }

    public void ResetRule()
    {
        if (!ruleTriggered) return;

        foreach (var input in inputsToActivate)
            input.StopGenerating();

        ruleTriggered = false;
    }
}
