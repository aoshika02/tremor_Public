using UnityEngine;

public class KeySE : MonoBehaviour, OnTriggerEnterInterface
{
    [SerializeField] private bool isOneShot = true;

    public void CallOnTriggerEnter(Collider collider, CallOnTrigger _)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            if (/*InGameFlow.Instance.IsGetKeyItem && */isOneShot)
            {
                
                SoundManager.Instance.PlaySE(SEType.piano_sound,transform);
                isOneShot = false;
            }
        }
    }
}
