/*
using UnityEngine;
public class AreaLight : AreaEventBase
{
    [SerializeField] private ObjectLight _objectLight;
    protected override void PlayerEnterEvent()
    {
        //TODO:ライトの入手処理
        _objectLight.LightOnPlayerHand();
        TutorialFlow.Instance.GetLight();
        ClearEvent();
    }

    public override void StartEvent()
    {
        base.StartEvent();
        _objectLight.OutlineView();
    }

    public override void ClearEvent()
    {
        base.ClearEvent();
        _objectLight.OutlineHide();
    }
}
*/
