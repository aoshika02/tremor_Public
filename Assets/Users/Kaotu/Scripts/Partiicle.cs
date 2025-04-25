using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Partiicle : MonoBehaviour
{
    [SerializeField]
    ParticleSystem _particleSystem;

    public void OnParticleSystemStopped()
    {
        _particleSystem.Stop(); 
    }
}
