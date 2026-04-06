using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MoleculeMovement : MonoBehaviour
{
    private AnimatedPipeController pipe;

    public void StartMovement(Transform objective)
    {
        transform.DOMove(objective.position, 2f).OnComplete(() => 
        { 
            pipe.SetCanStartBloodFlow(true);
            Destroy(gameObject); 
        });
    }

    public void SetPipe(AnimatedPipeController pipe)
    {
        this.pipe = pipe;
    }
}
