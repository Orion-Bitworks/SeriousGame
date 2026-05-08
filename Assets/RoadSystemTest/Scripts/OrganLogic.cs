using UnityEngine;

public class OrganLogic : MonoBehaviour
{
    [Header("Reglas del órgano")]
    public OrganRule[] rules;

    [Header("Animación del órgano")]
    [SerializeField] HeartAnimationController heartAnimation;

    bool animationStarted = false;

    public void NotifyOutputReceivedBall()
    {
        // Arrancar animación si aún no ha empezado
        if (!animationStarted)
        {
            animationStarted = true;
            AudioController.Instance.PlayHeartbeatOnce();
            heartAnimation.StartAnimation();
        }

        AudioController.Instance.PlaySFX(SFX.ThreeD, (int)HeartSFX.ParticleInOut);

        // Ejecutar reglas del órgano
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
        animationStarted = false;
        AudioController.Instance.StopHeartbeat();
        heartAnimation.StopAnimation();

        foreach (var rule in rules)
            rule.ResetRule();
    }
}