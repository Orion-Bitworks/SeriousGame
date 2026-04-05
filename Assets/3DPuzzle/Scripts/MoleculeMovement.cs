using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MoleculeMovement : MonoBehaviour
{
    public void StartMovement(Transform objective)
    {
        transform.DOMove(objective.position, 2f).OnComplete(() => { Destroy(gameObject); });
    }
}
