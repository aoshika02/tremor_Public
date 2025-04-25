using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Debug_GetItemIconViewManager : MonoBehaviour
{
    [SerializeField] private ItemEventName _itemEventName;

    [SerializeField] private ItemEventData _data;

    private ItemEvent GetEvent(ItemEventName name) => _data.DataList.FirstOrDefault(d => d.ItemEventName == name);

    [ContextMenu("AddTest")]
    private void AddTest()
    {
        var item = GetEvent(_itemEventName);
        GetItemIconViewManager.Instance.AddItemView(item);
    }

    [ContextMenu("RemoveTest")]
    private void RemoveTest()
    {
        GetItemIconViewManager.Instance.RemoveItemView(_itemEventName);
    }

    private async void Start()
    {
        GetItemIconViewManager.Instance.AddItemView(GetEvent(ItemEventName.Quiz_Memo));
        GetItemIconViewManager.Instance.AddItemView(GetEvent(ItemEventName.NewsPaper));
        GetItemIconViewManager.Instance.AddItemView(GetEvent(ItemEventName.Saw_Blade));
        GetItemIconViewManager.Instance.AddItemView(GetEvent(ItemEventName.TestItem1));

        await UniTask.Delay(TimeSpan.FromSeconds(3));
        GetItemIconViewManager.Instance.AddItemView(GetEvent(ItemEventName.Saw_Hand));
        GetItemIconViewManager.Instance.RemoveItemView(ItemEventName.Saw_Hand);
        GetItemIconViewManager.Instance.RemoveItemView(ItemEventName.Saw_Blade);
        GetItemIconViewManager.Instance.AddItemView(GetEvent(ItemEventName.Saw));
    }

    private void OnGUI()
    {
        if (GUILayout.Button("AddTest"))
        {
            AddTest();
        }

        if (GUILayout.Button("RemoveTest"))
        {
            RemoveTest();
        }
    }
}