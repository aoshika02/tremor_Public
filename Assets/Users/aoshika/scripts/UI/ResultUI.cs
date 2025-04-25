using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultUI : SingletonMonoBehaviour<ResultUI>
{
    private GhostType _ghostType;
    private CursedItemType _cursedItemType;
    //todo:別のスクリプトから_timeの実数値とGhostType、CursedItemTypeのtypeを取得
    private float _time = 305;
    private int _min;
    private int _sec;

    //値によって表示するテキストを変更
    [SerializeField] private int _ghostTypeValue = 1;
    [SerializeField] private int _cursedItemTypeValue = 1;

    private Dictionary<GhostType, string> GhostDic = new Dictionary<GhostType, string>();
    private Dictionary<CursedItemType, string> CursedItemDic = new Dictionary<CursedItemType, string>();
    [SerializeField] private TextMeshProUGUI _resultTime;
    [SerializeField] private TextMeshProUGUI _resultGhost;
    [SerializeField] private TextMeshProUGUI _resultItem;

    void Start()
    {
        AddGhostDic();
        AddCursedItemDic();
    }

    public void ClesrResultView()
    {
        /*
         見つけたゴーストの種類
         持ち帰った呪物
         クリアタイム
         */
        SetTime();
        _resultTime.text = $"ClearTime : {_min} : {_sec.ToString("d2")}";
        _resultGhost.text = $"FoundGhost : {GhostDic[(GhostType)_ghostTypeValue]}";
        _resultItem.text = $"GotItem : {CursedItemDic[(CursedItemType)_cursedItemTypeValue]}";
    }

    private void AddGhostDic()
    {
        GhostDic.Add(GhostType.Ghost0, "GhostA");
        GhostDic.Add(GhostType.Ghost1, "GhostB");
        GhostDic.Add(GhostType.Ghost2, "GhostC");
        GhostDic.Add(GhostType.Ghost3, "GhostD");
    }

    private void AddCursedItemDic()
    {
        CursedItemDic.Add(CursedItemType.Item0, "ItemA");
        CursedItemDic.Add(CursedItemType.Item1, "ItemB");
        CursedItemDic.Add(CursedItemType.Item2, "ItemC");
        CursedItemDic.Add(CursedItemType.Item3, "ItemD");
    }

    private void SetTime()
    {
        _min = (int)_time / 60;
        _sec = (int)_time % 60;
    }
}
