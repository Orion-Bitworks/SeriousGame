using UnityEngine;

public class HeartAnimationController : MonoBehaviour
{
    Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
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