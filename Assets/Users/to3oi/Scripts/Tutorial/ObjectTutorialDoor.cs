using UnityEngine;

public class ObjectTutorialDoor : OpenDoor,IOutlineViewer
{
    /*private bool _canOpen = false;
    public void OutlineView()
    {
        foreach (var transform in GetComponentsInChildren<Transform>())
        {
            transform.gameObject.layer = LayerMask.NameToLayer("Outline");
        }
    }

    public void OutlineHide()
    {
        foreach (var transform in GetComponentsInChildren<Transform>())
        {
            transform.gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    public override void DoorOpenAction(float x)
    {
        if(!_canOpen){return;}
        if (!( x == 1 && _isDoor)){return;}
        
        OutlineHide();
        base.DoorOpenAction(x);
        TutorialFlow.Instance.OpenDoor();
    }

    public void canOpen()
    {
        _canOpen = true;
    }*/
}
