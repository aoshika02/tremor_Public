using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ChasePoint : MonoBehaviour,OnTriggerEnterInterface,OnTriggerExitInterface
{
    [SerializeField] private Transform _ghostVoicePoint;
    [SerializeField] private Transform _ghostInitPosition;
    [SerializeField] private Transform _ghostChasePosition;
    [SerializeField] private Transform _ghostLookPoint;

    private bool _isGetCursedItemFirst = false;
    private bool _isChasePreparationFirst = false;
    public void CallOnTriggerEnter(Collider other, CallOnTrigger _)
    {
        if (InGameFlow.Instance.IsGetCursedItem && !_isGetCursedItemFirst)
        {
            _isGetCursedItemFirst = true;
            // 準備処理
            InGameFlow.Instance.ChasePreparationFlag(_ghostInitPosition);
            // ゴーストが近くにいる演出を入れたい
            SoundManager.Instance.PlaySE(SEType.GhostVoice04,_ghostVoicePoint);
        }
    }

    public void CallOnTriggerExit(Collider other, CallOnTrigger _)
    {
        if (InGameFlow.Instance.IsChasePreparation && !_isChasePreparationFirst)
        {
            _isChasePreparationFirst = true;
            // 開始処理
            InGameFlow.Instance.ChaseStartFlag(_ghostChasePosition);
            UniTask.Create(async () =>
            {
                PlayerMove.Instance.InvertMoveLockFlag();
                await PlayerMove.Instance.SetPlayerRotation(_ghostLookPoint,1f);
                await UniTask.Delay(TimeSpan.FromSeconds(2f));
                PlayerMove.Instance.InvertMoveLockFlag();
                PlayerMove.Instance.StartChaseFlag();
            }).Forget();
        }
    }
}
