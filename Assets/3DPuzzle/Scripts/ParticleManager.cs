using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;

    private Dictionary<string, ParticleSystem> particleDictionary = new Dictionary<string, ParticleSystem>();

    [Header("Particle Prefabs")]
    [SerializeField] private List<ParticleSystem> particles;

    private void Awake()
    {
        instance = this;

        foreach (ParticleSystem particle in particles)
        {
            particleDictionary.Add(particle.name, particle);
        }
    }

    public void SpawnParticles(string name, Transform position)
    {
        if (particleDictionary.ContainsKey(name))
        {
            Instantiate(particleDictionary[name], position);
        }
        else
        {
            Debug.LogError("No existen particulas con el identificador " + name);
        }
    }
}
