using TMPro;
using UnityEngine;
using UniRx;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class MessageViewEvent : SingletonMonoBehaviour<MessageViewEvent>
{
    [SerializeField] private MessageEventData _messageEventData;
    [SerializeField] private TextMeshProUGUI _playersText;
    [SerializeField] private TextMeshProUGUI _charaNameText;
    [SerializeField] private CanvasGroup _textCanvasGroup;
    [SerializeField] private CanvasGroup _textBackGround;

    [SerializeField] private CanvasGroup _charaNameGroups;

    //クイズ箱用
    [SerializeField] private Sprite _yesChoicesTex;
    [SerializeField] private Sprite _noChoicesTex;
    [SerializeField] private Image _yesChoicesImg;
    [SerializeField] private Image _noChoicesImg;
    [SerializeField] private TextMeshProUGUI _yesChoicesText;
    [SerializeField] private TextMeshProUGUI _noChoicesText;
    [SerializeField] private CanvasGroup _yesChoicesCanvasGroup;

    [SerializeField] private CanvasGroup _noChoicesCanvasGroup;

    //入力待機フラグ
    private bool _isDisp = false;

    //非表示処理中フラグ
    private bool _isClear = false;

    private void Start()
    {
        _playersText.text = "";
        _charaNameText.text = "";
        _textCanvasGroup.alpha = 0;
        _textBackGround.alpha = 0;
        _charaNameGroups.alpha = 0;
        _yesChoicesCanvasGroup.alpha = 0;
        _noChoicesCanvasGroup.alpha = 0;

       
        _yesChoicesImg.sprite = _yesChoicesTex;
        _noChoicesImg.sprite = _noChoicesTex;
        InputManager.Instance.Decision.Subscribe(async x =>
        {
            if (x != 1)
            {
                return;
            }

            if (_isClear) return;
            if (_isDisp) return;
            _isDisp = true;
        }).AddTo(this);
    }

    /// <summary>
    /// </summary>
    /// <param name="dispMessage">表示させる文字列</param>
    /// <param name="inOrder">一文字ずつ出すかどうかのフラグ</param>
    /// <param name="isClear">表示後自動で消すかどうかのフラグ</param>
    /// <param name="isFadeAll">表示後背景を消すかどうかのフラグ</param>
    /// <param name="dispTime">一定時間表示したい場合</param>
    /// <param name="clearTime">自動表示後消すのにかかる時間</param>
    /// <param name="isTalk">対話型か</param>
    /// <param name="isTalkEnd">表示後対話を終えるか</param>
    /// <param name="charaName">キャラ名</param>
    /// <returns></returns>
    public async UniTask SetText(string dispMessage,
        bool inOrder = true,
        bool isClear = false,
        bool isFadeAll = true,
        float dispTime = 0,
        float clearTime = 0.5f,
        bool isTalk = false,
        bool isTalkEnd = false,
        string charaName = "",
        bool IsWaitSE = false,
        float WaitSETime = 0
    )
    {
        //アルファが0なら表示する
        if (_textCanvasGroup.alpha == 0) await ValueChange(0, 1, 0.5f, _textCanvasGroup);
        if (_textBackGround.alpha == 0) await ValueChange(0, 1, 0.5f, _textBackGround);
        if (isTalk)
        {
            _charaNameText.text = charaName;
            if (_charaNameGroups.alpha == 0) await ValueChange(0, 1, 0.5f, _charaNameGroups);
        }

        if (inOrder)
        {
            //一文字ずつ表示させる処理
            var length = dispMessage.Length;
            _playersText.maxVisibleCharacters = 0;
            _playersText.text = dispMessage;
            for (var i = 0; i <= length; i++)
            {
                _playersText.maxVisibleCharacters = i;
                await UniTask.WaitForSeconds(0.1f, cancellationToken: destroyCancellationToken);
            }
        }
        else
        {
            _playersText.text = dispMessage;
        }

        if (IsWaitSE)
        {
            dispTime = Mathf.Max(dispTime, WaitSETime);
        }
        //[dispTime]秒だけ表示待機
        await UniTask.WaitForSeconds(dispTime, cancellationToken: destroyCancellationToken);
        
        //自動で消すかどうかの分岐
        _isDisp = false;
        if (isClear || IsWaitSE)
        {
            //入力待機フラグ無効化
            _isDisp = true;
        }
        //自動で消さない場合入力待機
        await UniTask.WaitUntil(() => _isDisp);

        await ClearText(clearTime, isClear, isFadeAll, isTalkEnd);
    }

    /// <summary>
    /// 非表示処理
    /// </summary>
    /// <param name="isFadeAll"></param>
    /// <param name="isTalkEnd"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private async UniTask ClearText(float duration, bool isClear, bool isFadeAll, bool isTalkEnd)
    {
        _isClear = true;

        List<UniTask> fadetask = new List<UniTask>();

        if (isFadeAll)
        {
            //すべて非表示
            fadetask.Add(ValueChange(_textBackGround.alpha, 0, 0.5f, _textBackGround));
            fadetask.Add(ValueChange(_charaNameGroups.alpha, 0, 0.5f, _charaNameGroups));
        }
        else
        {
            if (isTalkEnd)
            {
                //名前部分のみ非表示
                fadetask.Add(ValueChange(_charaNameGroups.alpha, 0, 0.5f, _charaNameGroups));
            }
        }

        //非表示処理が追加されたときのみ実行
        fadetask.Add(ValueChange(_textCanvasGroup.alpha, 0, 0.5f, _textCanvasGroup));
        await UniTask.WhenAll(fadetask);
        if (isTalkEnd) _charaNameText.text = "";
        _playersText.text = "";
        if (isClear)
        {
            await UniTask.WaitForSeconds(duration, cancellationToken: destroyCancellationToken);
        }
        else
        {
            await UniTask.Yield(cancellationToken: destroyCancellationToken);
        }

        _isClear = false;
    }

    /// <summary>
    /// 選択肢の表示
    /// </summary>
    /// <param name="end">目的の値</param>
    /// <param name="duration">変化時間</param>
    /// <param name="choiceType"></param>
    /// <returns></returns>
    public async UniTask ChoicesView(float end, float duration, ChoiceType choiceType, SelectElementType selectType)
    {
        if (selectType == SelectElementType.OpenBox)
        {
            _yesChoicesText.text = LocalizeText.GetText("UI_Text_OpenBox_Yes");
            _noChoicesText.text = LocalizeText.GetText("UI_Text_OpenBox_No");
        }
        if(selectType == SelectElementType.QuizItems) 
        {
            _yesChoicesText.text = LocalizeText.GetText("UI_Text_QuizItem_Memo");
            _noChoicesText.text = LocalizeText.GetText("UI_Text_QuizItem_Radio");
        }
        if (choiceType == ChoiceType.All)
        {
            UniTask[] tasks = new UniTask[2];
            tasks[0] = ValueChange(_yesChoicesCanvasGroup.alpha, end, duration, _yesChoicesCanvasGroup);
            tasks[1] = ValueChange(_yesChoicesCanvasGroup.alpha, end, duration, _noChoicesCanvasGroup);
            await UniTask.WhenAll(tasks);
            return;
        }

        if (choiceType == ChoiceType.Yes)
        {
            await ValueChange(_yesChoicesCanvasGroup.alpha, end, duration, _noChoicesCanvasGroup);

            await UniTask.WaitForSeconds(0.5f, cancellationToken: destroyCancellationToken);

            await ValueChange(_yesChoicesCanvasGroup.alpha, end, duration, _yesChoicesCanvasGroup);
            return;
        }

        if (choiceType == ChoiceType.No)
        {
            await ValueChange(_yesChoicesCanvasGroup.alpha, end, duration, _yesChoicesCanvasGroup);

            await UniTask.WaitForSeconds(0.5f, cancellationToken: destroyCancellationToken);

            await ValueChange(_yesChoicesCanvasGroup.alpha, end, duration, _noChoicesCanvasGroup);
            return;
        }
    }

    private async UniTask ValueChange(float start, float end, float duration, CanvasGroup canvasGroup)
    {
        await DOVirtual.Float(start, end, duration, f => { canvasGroup.alpha = f; })
            .ToUniTask(cancellationToken: destroyCancellationToken);
    }

    public async UniTask ViewText(MessageEventName name)
    {
        SoundHash soundHash = null;
        List<(BGMType BGMType, SoundHash SoundHash)> bgmSoundHashes = new List<(BGMType, SoundHash)>();
        var messageEvent = GetMessageData(name);
        if (messageEvent == null)
        {
            Debug.LogError($"Message Event is null :Event Name => {name}");
            return;
        }

        for (int i = 0; i < messageEvent.MessageKey.Count; i++)
        {
            var data = messageEvent.MessageKey[i];
            Debug.Log($"{LocalizeText.GetText(data.DisplayMessageKey)}");
            if (data.IsPlaySE == true)
            {
                soundHash = SoundManager.Instance.PlaySE(data.SEType, data.SEVolume);
            }

            if (data.IsPlayBGM == true)
            {
                bgmSoundHashes.Add((data.BGMType, SoundManager.Instance.PlayBGM(data.BGMType, data.BGMVolume)));
            }

            await SetText(LocalizeText.GetText(data.DisplayMessageKey),
                data.InOrder,
                data.IsClear,
                i == messageEvent.MessageKey.Count - 1 ? true : data.IsFadeBackGround,
                data.DisplayTime,
                data.ClearTime,
                data.IsTalk,
                data.IsTalkEnd,
                "",
                data.IsWaitSE,
                soundHash?.GetClipTime ?? 0); //LocalizeText.GetText(data.CharaNameKey));
            if (data.IsPlaySE && soundHash != null) SoundManager.Instance.StopSE(soundHash, 0).Forget();
            if (data.IsStopBGM)
            {
                var t = bgmSoundHashes.FirstOrDefault(x => x.BGMType == data.BGMType);
                if (t.SoundHash != null)
                {
                    SoundManager.Instance.StopBGM(t.SoundHash, 0).Forget();
                    bgmSoundHashes.Remove(t);
                }
            }
        }

        foreach ((BGMType BGMType, SoundHash SoundHash) sh in bgmSoundHashes)
            SoundManager.Instance.StopBGM(sh.SoundHash).Forget();
    }

    MessageEvent GetMessageData(MessageEventName keyName) =>
        _messageEventData.DataList.FirstOrDefault(x => x.MessageEventName == keyName);
}

public enum ChoiceType
{
    All = 0,
    Yes = 1,
    No = 2,
}

public enum SelectElementType
{
    OpenBox=0,
    QuizItems=1,
}