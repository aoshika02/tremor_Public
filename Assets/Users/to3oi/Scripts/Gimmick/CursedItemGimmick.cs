using UniRx;
using UnityEngine;

public class CursedItemGimmick : MonoBehaviour,OnTriggerEnterInterface,OnTriggerExitInterface
{
    protected bool _isTriggerEnter = false;
    [SerializeField] private GameObject _cursedItem;
    private void Start()
    {
        InputManager.Instance.Decision.Subscribe(async x =>
        {
            if(_isTriggerEnter == false) return;
            if(x != 1) return;
            
            PlayerMove.Instance.SetMove(false);
            //呪物を入手したときの演出
            await FadeInOut.FadeIn();
            InGameFlow.Instance.CursedItemFlag();
            _cursedItem.SetActive(false);
            //SE挿入
            SoundManager.Instance.PlaySE(SEType.Get_Heart_SE);
            await FadeInOut.FadeOut();
            PlayerMove.Instance.SetMove(true);

        }).AddTo(this);
    }

    public void CallOnTriggerEnter(Collider collider, CallOnTrigger _)
    {
        _isTriggerEnter = true;
    }

    public void CallOnTriggerExit(Collider collider, CallOnTrigger _)
    {
        _isTriggerEnter = false;
    }
}
