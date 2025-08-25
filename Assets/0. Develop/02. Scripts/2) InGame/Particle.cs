using System;
using UnityEngine;

public class Particle : PoolObject
{
    [SerializeField] private ParticleSystem particleSystem;
    private void OnDisable()
    {
        particleSystem.Stop();
    }
}
