using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// このスクリプトはメインのパーティクルシステムオブジェクトにアタッチすることを想定している
[RequireComponent(typeof(ParticleSystem))]
public class DeathDetector : MonoBehaviour
{
    public OnDeathEvent onDeath;

    private new ParticleSystem particleSystem;
    private GameObject deathDetectorEmitter;
    private ParticleSystem deathDetectorParticleSystem;
    private ParticleSystem.Particle[] particles;

    private void Awake()
    {
        if (this.onDeath == null)
        {
            this.onDeath = new OnDeathEvent();
        }
    }

    private void OnEnable()
    {
        // メインの子として、Burstでパーティクルを1発だけ発射するパーティクルシステムを作る
        this.particleSystem = this.GetComponent<ParticleSystem>();
        this.deathDetectorEmitter = new GameObject("Death Detector Emitter");
        this.deathDetectorEmitter.transform.SetParent(this.transform, false);
        this.deathDetectorParticleSystem = this.deathDetectorEmitter.AddComponent<ParticleSystem>();
        var shapeModule = this.deathDetectorParticleSystem.shape;
        shapeModule.enabled = false;
        var rendererModule = this.deathDetectorParticleSystem.GetComponent<ParticleSystemRenderer>();
        rendererModule.enabled = false;
        var mainModule = this.deathDetectorParticleSystem.main;
        mainModule.startSpeed = 0;
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;
        var emissionModule = this.deathDetectorParticleSystem.emission;
        emissionModule.rateOverTime = 0;
        emissionModule.SetBursts(new[] { new ParticleSystem.Burst(0, 1) });

        // それをメインのサブエミッター(Death)として登録する
        var subEmittersModule = this.particleSystem.subEmitters;
        subEmittersModule.AddSubEmitter(
            this.deathDetectorParticleSystem,
            ParticleSystemSubEmitterType.Death,
            ParticleSystemSubEmitterProperties.InheritNothing);

        // パーティクル取得用の配列を確保
        this.particles = new ParticleSystem.Particle[mainModule.maxParticles];
    }

    private void OnDisable()
    {
        // サブエミッターの登録を解除し、オブジェクトも破壊する
        var subEmittersModule = this.particleSystem.subEmitters;
        foreach (var i in Enumerable.Range(0, subEmittersModule.subEmittersCount)
            .Select(i => (i, subEmittersModule.GetSubEmitterSystem(i)))
            .Where(ps => ps.Item2 == this.deathDetectorParticleSystem).Select(ps => ps.Item1).Reverse())
        {
            subEmittersModule.RemoveSubEmitter(i);
        }

        Destroy(this.deathDetectorEmitter);
        this.particleSystem = null;
        this.deathDetectorEmitter = null;
        this.deathDetectorParticleSystem = null;
        this.particles = null;
    }

    private void LateUpdate()
    {
        // 消滅検出用サブエミッターのパーティクル数を監視し...
        if ((this.deathDetectorParticleSystem.particleCount > 0) && (this.onDeath != null))
        {
            // パーティクルが出現していればデータを取得する
            var count = this.deathDetectorParticleSystem.GetParticles(this.particles);
            for (var i = 0; i < count; i++)
            {
                // 寿命を0にすることでパーティクルを削除させ...
                this.particles[i].remainingLifetime = 0;

                // パーティクルの座標を添えてイベントを発火させる
                this.onDeath.Invoke(this.particles[i].position);
            }

            // データを書き戻す
            this.deathDetectorParticleSystem.SetParticles(this.particles, count);
        }
    }

    [Serializable]
    public class OnDeathEvent : UnityEvent<Vector3>
    {
    }
}
