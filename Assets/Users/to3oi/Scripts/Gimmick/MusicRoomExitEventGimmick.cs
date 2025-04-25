using Cysharp.Threading.Tasks;
using UnityEngine;

public class MusicRoomExitEventGimmick : MonoBehaviour,OnTriggerEnterInterface
{
    [SerializeField] private GameObject _suzu;
   
    public void CallOnTriggerEnter(Collider other, CallOnTrigger callOnTrigger)
    {
        callOnTrigger.ActionComplete();
        EventStart().Forget();
    }

    private async UniTask EventStart()
    {
        // プレイヤーの懐中電灯を一瞬消す
        await Player.Instance.Flashlight(false,0.5f);

        // 女の子を消す
        _suzu.SetActive(false);

        // プレイヤーの懐中電灯をつける
        await Player.Instance.Flashlight(true,0.5f);
    }
}
