using UnityEngine;

public class Debug_NoiseParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private ParticleSystem _particleSystem2;


    private void OnGUI()
    {
        if (GUILayout.Button("Particle Play"))
        {
            _particleSystem?.Play();
            _particleSystem2?.Play();
        }

        if (GUILayout.Button("Particle Stop"))
        {
            _particleSystem?.Stop();
            _particleSystem2?.Stop();
        }
    }
}
