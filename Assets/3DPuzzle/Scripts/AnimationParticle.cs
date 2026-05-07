using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationParticle : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particles;

    public void StartParticle(int index)
    {
        if (particles.Count - 1 < index || particles[index] == null) return;

        particles[index].Play();
    }
}
