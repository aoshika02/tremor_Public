using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// ���̃X�N���v�g�̓��C���̃p�[�e�B�N���V�X�e���I�u�W�F�N�g�ɃA�^�b�`���邱�Ƃ�z�肵�Ă���
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
        // ���C���̎q�Ƃ��āABurst�Ńp�[�e�B�N����1���������˂���p�[�e�B�N���V�X�e�������
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

        // ��������C���̃T�u�G�~�b�^�[(Death)�Ƃ��ēo�^����
        var subEmittersModule = this.particleSystem.subEmitters;
        subEmittersModule.AddSubEmitter(
            this.deathDetectorParticleSystem,
            ParticleSystemSubEmitterType.Death,
            ParticleSystemSubEmitterProperties.InheritNothing);

        // �p�[�e�B�N���擾�p�̔z����m��
        this.particles = new ParticleSystem.Particle[mainModule.maxParticles];
    }

    private void OnDisable()
    {
        // �T�u�G�~�b�^�[�̓o�^���������A�I�u�W�F�N�g���j�󂷂�
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
        // ���Ō��o�p�T�u�G�~�b�^�[�̃p�[�e�B�N�������Ď���...
        if ((this.deathDetectorParticleSystem.particleCount > 0) && (this.onDeath != null))
        {
            // �p�[�e�B�N�����o�����Ă���΃f�[�^���擾����
            var count = this.deathDetectorParticleSystem.GetParticles(this.particles);
            for (var i = 0; i < count; i++)
            {
                // ������0�ɂ��邱�ƂŃp�[�e�B�N�����폜����...
                this.particles[i].remainingLifetime = 0;

                // �p�[�e�B�N���̍��W��Y���ăC�x���g�𔭉΂�����
                this.onDeath.Invoke(this.particles[i].position);
            }

            // �f�[�^�������߂�
            this.deathDetectorParticleSystem.SetParticles(this.particles, count);
        }
    }

    [Serializable]
    public class OnDeathEvent : UnityEvent<Vector3>
    {
    }
}
