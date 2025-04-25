using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PovTest : MonoBehaviour
{
    PlayerMove _playerMove;
    [SerializeField] private Transform _transform;
    async void Start()
    {
        CancelToken _cancelToken = new CancelToken();
        _playerMove = PlayerMove.Instance;
        await UniTask.WaitForSeconds(5);
        VibrationQuiz.Instance.StartVibrationQuizFlow(_cancelToken.GetToken());

        //FovTest();
    }

    async void FovTest()
    {
        _playerMove.InvertMoveLockFlag();
        await _playerMove.SetPlayerRotation(_transform, 2);
        await UniTask.WaitForSeconds(2);
        _playerMove.InvertMoveLockFlag();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            VibrationQuiz.Instance.CheckMemoFlag();
            VibrationQuiz.Instance.CheckRadioFlag();
        }
    }
}
