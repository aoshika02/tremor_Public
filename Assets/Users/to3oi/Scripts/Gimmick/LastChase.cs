using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class LastChase : MonoBehaviour, OnTriggerEnterInterface
{
    [SerializeField] private List<ObstaclesData> _obstaclesDatas = new List<ObstaclesData>();

    [Serializable]
    public class ObstaclesData
    {
        public List<Transform> ObstaclesPositions = new List<Transform>();
    }

    [SerializeField] private GameObject[] _rodkPrefabs;
    [SerializeField] private GameObject[] _deskPrefabs;

    [SerializeField] private float _startYPosition = 4f;
    [SerializeField] private float _endYPosition = 0f;
    [SerializeField] private float _effectYPosition = 4f;

    [SerializeField] private ParticleSystem _noiseParticlePrefab;
    private int _index;

    // 仕様 
    // ゴーストの変身シーンが終わったら呼び出す
    // 3レーン想定で、どれかのレーンに障害物が降ってくるのでそれをいい感じに避けて進むタイプのギミック
    //　障害物が降るたびにカメラをシェイクさせる
    // 障害物がフル場所はランダムでも良い

    public void CallOnTriggerEnter(Collider collider, CallOnTrigger callOnTrigger)
    {
        if(InGameFlow.Instance.IsLastChase is false){return;}
        
        callOnTrigger.ActionComplete();
        FallTask().Forget();
    }

    private async UniTask FallTask()
    {
        // 初期化
        var t = GetObstaclesPosition(_index);
        _index++;

        // 落下物の種類 0 = Rock, 1 = Desk    
        int type = Random.Range(0, 2);
        var prefab = type switch
        {
            0 => GetRockPrefab(),
            1 => GetDeskPrefab(),
            _ => throw new ArgumentOutOfRangeException()
        };

        var g = Instantiate(prefab, t.position, quaternion.identity);
        g.transform.DOMoveY(_startYPosition, 0);

        /*
        // TODO:モヤのエフェクトを表示する
        var e = Instantiate(_noiseParticlePrefab,Vector3.zero, quaternion.identity,g.transform);
        e.transform.localPosition = Vector3.zero;

        */
        // TODO:机のときはアニメーションさせる必要がありそう
        await g.transform.DOMoveY(_endYPosition, Random.Range(0.5f, 0.1f)).SetEase(Ease.InQuint).ToUniTask();

        switch (type)
        {
            case 0:
            {
                SoundManager.Instance.PlaySE(SEType.FallRock_SE);
            }
                break;
            case 1:
            {
                SoundManager.Instance.PlaySE(SEType.FallDesk_SE);
            }
                break;
        }
        PlayerMove.Instance.ShakeCamera(0.2f, 2, 10).Forget();
    }

    private Transform GetObstaclesPosition(int index)
    {
        return _obstaclesDatas[index].ObstaclesPositions[Random.Range(0, 3)];
    }

    private GameObject GetRockPrefab()
    {
        return _rodkPrefabs[Random.Range(0, _rodkPrefabs.Length)];
    }

    private GameObject GetDeskPrefab()
    {
        return _deskPrefabs[Random.Range(0, _deskPrefabs.Length)];
    }
}