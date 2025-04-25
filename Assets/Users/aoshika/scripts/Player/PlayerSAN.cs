using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSAN : SingletonMonoBehaviour<PlayerSAN>
{
    Player _player;
    PlayerStatus _playerStatus;

    private float _distance;

    //SAN値の増え具合
    [SerializeField] private float _addSAN;

    [SerializeField] private List<GhostPlayerDistance> _impactTable = new List<GhostPlayerDistance>()
    {
        //LowerDistance：ゴーストの影響を受けるゴーストとの距離の境界
        //DeclineRate：SAN値の減り具合（秒間）
        new GhostPlayerDistance() { LowerDistance = 6, DeclineRate = 20, },
        new GhostPlayerDistance() { LowerDistance = 10, DeclineRate = 10, },
        new GhostPlayerDistance() { LowerDistance = 15, DeclineRate = 8, },
    };

    [Serializable]
    private class GhostPlayerDistance
    {
        ////SAN値の減らし具合が変わるそれぞれの距離
        public float LowerDistance = 0;

        ////SAN値の減り具合
        public float DeclineRate = 0;
    }

    List<Transform> _enemyTransforms = new List<Transform>();

    private void Start()
    {
        _player = Player.Instance;
        _playerStatus = PlayerStatus.Instance;
    }

    public void AddEnemyTransform(Transform transform)
    {
        if (_enemyTransforms.Find(x => transform == x) == null)
        {
            _enemyTransforms.Add(transform);
        }
    }

    public void RemoveEnemyTransform(Transform transform)
    {
        if (_enemyTransforms.Find(x => transform == x) != null)
        {
            _enemyTransforms.Remove(transform);
        }
    }

    private void Update()
    {
        _enemyTransforms.ForEach(UpdatePlayerSAN);

        if (_enemyTransforms.Count != 0) return;
        if (_playerStatus.San < 100)
        {
            _playerStatus.AddSAN(_addSAN * Time.deltaTime);
        }
    }

    private void UpdatePlayerSAN(Transform transform)
    {
        //距離を算出
        _distance = Vector3.Distance(_player.Position, transform.position);

        if (_distance <= _impactTable[0].LowerDistance)
        {
            _playerStatus.SubSAN(_impactTable[0].DeclineRate * Time.deltaTime);
        }
        else if (_distance <= _impactTable[1].LowerDistance)
        {
            _playerStatus.SubSAN(_impactTable[1].DeclineRate * Time.deltaTime);
        }
        else if (_distance <= _impactTable[2].LowerDistance)
        {
            _playerStatus.SubSAN(_impactTable[2].DeclineRate * Time.deltaTime);
        }
        else
        {
            _playerStatus.AddSAN(_addSAN * Time.deltaTime);
        }
    }
}