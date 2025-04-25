using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GetItemIconViewManager : SingletonMonoBehaviour<GetItemIconViewManager>
{
    public class ItemIcon
    {
        public ItemEventName ItemEventName;
        public GetItemIconView GetItemIconView;

        public ItemIcon(ItemEventName _itemEventName)
        {
            ItemEventName = _itemEventName;
        }
    }

    [SerializeField] private Transform _itemViewRoot;
    [SerializeField] private GameObject _getItemIconView;

    private List<ItemIcon> _itemList = new List<ItemIcon>();
    private const float _getItemIconViewHeight = 100f;

    public void AddItemView(ItemEvent itemEvent)
    {
        // nullなら終了
        if (itemEvent == null)
        {
            return;
        }

        // 入手済みなら追加でアイコンを表示しない
        if (_itemList.Any(x => x.ItemEventName == itemEvent.ItemEventName))
        {
            return;
        }
        var getItemIconView = Instantiate(_getItemIconView, _itemViewRoot).GetComponent<GetItemIconView>();

        var itemIcon = new ItemIcon(itemEvent.ItemEventName);
        itemIcon.GetItemIconView = getItemIconView;


        getItemIconView.SetUp(itemEvent.Texture2D,
            (_itemList.Count + 1) * _getItemIconViewHeight * -1,
            (_itemList.Count) * _getItemIconViewHeight * -1
        ).Forget();

        _itemList.Add(itemIcon);
    }

    public void RemoveItemView(ItemEventName itemEventName)
    {
        var itemIcon = _itemList.FirstOrDefault(x => x.ItemEventName == itemEventName);
        if (itemIcon == null)
        {
            return;
        }

        var moveTargetList = _itemList.SkipWhile(icon => icon != itemIcon).Skip(1).ToList();
        _itemList.Remove(itemIcon);
        itemIcon.GetItemIconView.Release();

        // アイテムの表示処理
        foreach (var moveItemIcon in moveTargetList)
        {
            var index = _itemList.IndexOf(moveItemIcon);
            moveItemIcon.GetItemIconView.Move(
                (index) * _getItemIconViewHeight * -1
            ).Forget();
        }
    }
}