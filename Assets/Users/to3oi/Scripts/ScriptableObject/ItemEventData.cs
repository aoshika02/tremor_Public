using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemEventData", menuName = "Data/ItemEventData")]
public class ItemEventData : ScriptableObject
{
    public List<ItemEvent> DataList = new List<ItemEvent>();
}

[Serializable]
public partial class ItemEvent
{
    public ItemEventType ItemEventType;
    public ItemEventName ItemEventName;
    public string ItemNameKey;
}

/// <summary>
/// Item
/// </summary>
public partial class ItemEvent
{
    [Space(10)]
    [Header("アイテム用変数")]
    public Texture2D Texture2D;
}

/// <summary>
/// Text
/// </summary>
public partial class ItemEvent
{
    [Space(10)]
    [Header("テキスト用変数")]
    public List<string> TextKey = new List<string>();
}
