/*using UnityEngine;

public class ObjectLight : MonoBehaviour, IOutlineViewer
{
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

    public void LightOnPlayerHand()
    {
        //TODO:適当な場所に移動
        transform.parent = Player.Instance.PayerLeftHandPosition;
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
}*/