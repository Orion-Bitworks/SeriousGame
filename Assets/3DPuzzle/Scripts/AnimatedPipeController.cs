using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnimatedPipeController : MonoBehaviour
{
    [SerializeField] GameObject entryMolecule;
    [SerializeField] GameObject exitMolecule;

    [SerializeField] Transform entry;
    [SerializeField] List<Transform> targets = new List<Transform>();
    [SerializeField] Transform exit;

    bool canStartBloodFlow = false;
    private bool stopSpawning = false;

    private Sequence spawnSequence;

    public void StartAnimation(bool inverted = false, bool blue = false)
    {
        spawnSequence?.Kill();

        spawnSequence = DOTween.Sequence();
        spawnSequence.AppendInterval(0.5f);
        spawnSequence.AppendCallback(() =>
        {
            if (stopSpawning)
            {
                return;
            }

            if (inverted && blue)
            {
                GameObject newMolecule = Instantiate(exitMolecule, exit.position, exit.rotation, entry);
                newMolecule.GetComponent<MoleculeMovement>().SetPipe(this);
                newMolecule.GetComponent<MoleculeMovement>().StartMovement(entry, targets);
            }
            else if (inverted && !blue)
            {
                GameObject newMolecule = Instantiate(entryMolecule, exit.position, exit.rotation, entry);
                newMolecule.GetComponent<MoleculeMovement>().SetPipe(this);
                newMolecule.GetComponent<MoleculeMovement>().StartMovement(entry, targets);
            }
            else if (!inverted && !blue)
            {
                GameObject newMolecule = Instantiate(entryMolecule, entry.position, entry.rotation, exit);
                newMolecule.GetComponent<MoleculeMovement>().SetPipe(this);
                newMolecule.GetComponent<MoleculeMovement>().StartMovement(exit, targets);
            }
            else if (!inverted && blue)
            {
                GameObject newMolecule = Instantiate(exitMolecule, entry.position, entry.rotation, exit);
                newMolecule.GetComponent<MoleculeMovement>().SetPipe(this);
                newMolecule.GetComponent<MoleculeMovement>().StartMovement(exit, targets);
            }
            
        });

        spawnSequence.SetLoops(-1, LoopType.Restart);
    }

    public bool CanStartBloodFlow()
    {
        return canStartBloodFlow;
    }

    public void SetCanStartBloodFlow(bool state)
    {
        canStartBloodFlow = state;
    }

    public void StopSpawning()
    {
        stopSpawning = true;

        if (spawnSequence != null && spawnSequence.IsActive())
        {
            spawnSequence.Kill();
        }
    }

    private void OnDestroy()
    {
        if (spawnSequence != null)
        {
            spawnSequence.Kill();
        }

        DOTween.Kill(transform);
        DOTween.Kill(gameObject);
    }
}
