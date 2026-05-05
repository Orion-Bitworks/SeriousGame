using UnityEngine;

public class HeartAnimationController : MonoBehaviour
{
    Animator animator;

    const float baseSpeed = 0.8f;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Velocidad = baseSpeed * velocityMultiplier (1, 2 o 3)
        animator.speed = baseSpeed * GameManager.Instance.velocityMultiplier;
    }

    public void StartAnimation()
    {
        animator.SetTrigger("Start");
    }

    public void StopAnimation()
    {
        animator.SetTrigger("Stop");
    }
}