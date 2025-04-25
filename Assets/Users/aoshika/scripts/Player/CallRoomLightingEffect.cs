using Cysharp.Threading.Tasks;
using UnityEngine;

public class CallRoomLightingEffect : MonoBehaviour
{
    private bool _inToSchool = false;
    [SerializeField]private bool _isStartRoom = false;

    private void Start()
    {
        if (!_isStartRoom) return;
        RoomLightingEffect.Instance.CallValueChange(true).Forget();
    }
    private void CallLightingValueChange() 
    {
        RoomLightingEffect.Instance.CallValueChange(_inToSchool).Forget();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            _inToSchool = true;
            CallLightingValueChange();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _inToSchool = false;
            CallLightingValueChange();
        }
    }
}
