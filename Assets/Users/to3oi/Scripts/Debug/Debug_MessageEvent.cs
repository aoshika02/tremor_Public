using Cysharp.Threading.Tasks;
using UnityEngine;

public class Debug_MessageEvent : MonoBehaviour
{
    [SerializeField] private MessageEventName name;

    async UniTask Start()
    {
        await MessageViewEvent.Instance.ViewText(name);
    }
}