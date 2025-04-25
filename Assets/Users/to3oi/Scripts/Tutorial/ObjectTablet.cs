using UnityEngine;

public class ObjectTablet : MonoBehaviour,IOutlineViewer
{
    private bool _isBreakTablet = false;
    public void BreakTablet()
    {
        if(_isBreakTablet){return;}
        _isBreakTablet = true;
        
        //TODO:シェーダーで画面を割る
        //現状タブレットが割れた状態のテクスチャを用意して差し替えるだけで大丈夫そう？
        //要検討
    }
    public void OutlineView()
    {
        gameObject.layer = LayerMask.NameToLayer("Outline");
    }
    
    public void OutlineHide()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
}
