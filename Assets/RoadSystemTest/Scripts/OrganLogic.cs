using UnityEngine;

public class OrganLogic : MonoBehaviour
{
    [Header("Reglas del órgano")]
    public OrganRule[] rules;

    public void NotifyOutputReceivedBall()
    {
        foreach (var rule in rules)
        {
            if (!rule.ruleTriggered && rule.CheckCondition())
            {
                rule.ExecuteRule();
            }
        }
    }

    public void ResetOrgan()
    {
        foreach (var rule in rules)
        {
            rule.ResetRule();
        }
    }
}