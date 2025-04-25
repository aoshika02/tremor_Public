using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Debug_GetItemEvent : MonoBehaviour
{
    [SerializeField] private ItemEventName name;

    async UniTask Start()
    {
        await GetItemEvent.Instance.ViewItem(name);
        //await UniTask.Delay(TimeSpan.FromSeconds(2));
        //await GetItemEvent.Instance.ViewItem(ItemEventName.TestItem2);
    }
}