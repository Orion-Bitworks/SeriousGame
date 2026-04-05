using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnimatedPipeController : MonoBehaviour
{
    [SerializeField] GameObject entryMolecule;
    [SerializeField] GameObject exitMolecule;

    [SerializeField] Transform entry;
    [SerializeField] Transform exit;

    public void StartAnimation(bool inverted = false)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.5f);
        sequence.AppendCallback(() =>
        {
            if (inverted)
            {
                GameObject newMolecule = Instantiate(exitMolecule, exit.position, exit.rotation, exit);
                newMolecule.GetComponent<MoleculeMovement>().StartMovement(entry);
            }
            else{
                GameObject newMolecule = Instantiate(entryMolecule, entry.position, entry.rotation, entry);
                newMolecule.GetComponent<MoleculeMovement>().StartMovement(exit);
            }
            
        });

        sequence.SetLoops(-1, LoopType.Restart);
    }
}
