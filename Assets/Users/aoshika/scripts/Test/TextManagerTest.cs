using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManagerTest : MonoBehaviour
{
    MessageViewEvent _messageViewEvent;
    void Start()
    {
        TestTestManagerFunc();
    }

    private async void TestTestManagerFunc() 
    {
        await UniTask.WaitForSeconds(1);
        _messageViewEvent = MessageViewEvent.Instance;
        await _messageViewEvent.SetText("入力待機",isFadeAll:false);
        await _messageViewEvent.SetText("入力待機名前", isFadeAll: false, isTalk: true,isTalkEnd:true, charaName: "はやて");
        await _messageViewEvent.SetText("入力待機",isFadeAll:false);
        await _messageViewEvent.SetText("入力待機名前", isTalk: true, charaName: "はやて");
        await _messageViewEvent.SetText("自動非表示", dispTime: 2,isClear:true, clearTime:2);
        await _messageViewEvent.SetText("自動非表示", isFadeAll: false, dispTime:2, isClear: true, clearTime: 2, isTalk: true, charaName: "はやて");
        await _messageViewEvent.SetText("自動非表示", dispTime: 2,isClear: true, clearTime:2, isTalkEnd: true);
        await _messageViewEvent.SetText("自動非表示", dispTime: 2, isClear: true, clearTime: 2, charaName: "はやて");


    }
}
