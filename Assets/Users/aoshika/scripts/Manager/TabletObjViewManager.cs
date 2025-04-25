using UnityEngine;

public class TabletObjViewManager : SingletonMonoBehaviour<TabletObjViewManager>
{
    [SerializeField] private Animator _tabletanim;
    private bool _isFirstCall = false;

    public void TabletObjActive() 
    {
        if (_isFirstCall is false)
        {
            _tabletanim.SetTrigger("isclose");
        }
        _tabletanim.SetBool("ischange", false);
    }
    public void TabletObjDisable() 
    {
        _tabletanim.SetBool("ischange",true);
    }
}
