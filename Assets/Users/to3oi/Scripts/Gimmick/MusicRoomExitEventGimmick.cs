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
        // �v���C���[�̉����d������u����
        await Player.Instance.Flashlight(false,0.5f);

        // ���̎q������
        _suzu.SetActive(false);

        // �v���C���[�̉����d��������
        await Player.Instance.Flashlight(true,0.5f);
    }
}
