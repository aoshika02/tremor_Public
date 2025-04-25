using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MessageEventData", menuName = "Data/MessageEventData")]
public class MessageEventData : ScriptableObject
{
    public List<MessageEvent> DataList;
}

[Serializable]
public class MessageEvent
{
    public MessageEventName MessageEventName;
    public List<MessageData> MessageKey;
}

[Serializable]
public class MessageData
{
    public string DisplayMessageKey;
    public bool InOrder = true;
    public bool IsClear = false;
    public bool IsFadeBackGround = true;
    public float DisplayTime = 0;
    public float ClearTime = 0.5f;
    public bool IsTalk = false;
    public bool IsTalkEnd = false;
    public string CharaNameKey = "";
    public bool IsPlaySE = false;
    public SEType SEType;
    public bool IsPlayBGM = false;
    public bool IsStopBGM = false;
    public float SEVolume = 1;
    public float BGMVolume = 1;
    public BGMType BGMType;
    public bool IsWaitSE = false;
}