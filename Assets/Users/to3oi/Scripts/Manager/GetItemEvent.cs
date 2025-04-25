using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UniRx;

public class GetItemEvent : SingletonMonoBehaviour<GetItemEvent>
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private ItemEventData _data;

    [SerializeField] private float _moveDuration = 0.25f;
    [SerializeField] private float _moveDelayDuration = 0.15f;

    [Header("アイテム表示用変数")]
    [SerializeField] private TextMeshProUGUI _itemName;
    [SerializeField] private RawImage _itemNameBG;
    [SerializeField] private RectTransform _viewAreaRoot;

    [Header("アイテム テクスチャ")] 
    [SerializeField] private RawImage _itemTexture;

    [Header("アイテム テキスト")]
    [SerializeField] private TextMeshProUGUI _itemText;
    [SerializeField] private RawImage _itemTextBG;
    [SerializeField] private CanvasGroup _itemTextCanvasGroup;
    [SerializeField] private float _itemTextCVGDuration = 0.5f;

    // 初期値を保持
    private Vector3 _itemNameBGPosition;
    private Vector3 _viewAreaRootPosition;

    void Start()
    {
        Reset();
    }

    public async UniTask ViewItem(ItemEventName name,bool viewIcon = true)
    {
        var itemEvent = GetEvent(name);
        if (itemEvent == null)
        {
            Debug.LogError($"{name} is null");
            return;
        }

        // プレイヤーの移動を停止
        PlayerMove.Instance?.SetMove(false);

        // イベントデータをセット 
        SetEventData(itemEvent);

        await InAnimation();
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        await ItemView(itemEvent);
        await TextView(itemEvent);
        await ExitAnimation();

        if (viewIcon)
        {
            // 入手したアイテムのアイコンを表示
            GetItemIconViewManager.Instance.AddItemView(itemEvent);
        }
        // プレイヤーの移動停止を解除
        PlayerMove.Instance?.SetMove(true);
    }

    private ItemEvent GetEvent(ItemEventName name) => _data.DataList.FirstOrDefault(d => d.ItemEventName == name);

    private void SetEventData(ItemEvent itemEvent)
    {
        // 必ず代入がある
        _itemName.text = LocalizeText.GetText(itemEvent.ItemNameKey);
        var isTexture = itemEvent?.Texture2D;
        _itemTexture.texture = itemEvent?.Texture2D;

        _itemTexture.color = isTexture != null ? Color.white : new Color(1, 1, 1, 0);

        switch (itemEvent.ItemEventType)
        {
            case ItemEventType.Item:
            {
            }
                break;
            case ItemEventType.Text:
            {
            }
                break;

            case ItemEventType.ItemAndText:
            {
            }
                break;
        }
    }

    private async UniTask ItemView(ItemEvent itemEvent)
    {
        if (itemEvent.ItemEventType != ItemEventType.Item &&
            itemEvent.ItemEventType != ItemEventType.ItemAndText)
        {
            return;
        }
        
        await WaitDecision();
    }
    private async UniTask TextView(ItemEvent itemEvent)
    {
        if (itemEvent.ItemEventType != ItemEventType.Text &&
            itemEvent.ItemEventType != ItemEventType.ItemAndText)
        {
            return;
        }

        // 初期化しておく
        _itemText.text = "";
        
        await _itemTextCanvasGroup.DOFade(1.0f, _itemTextCVGDuration);

        for(int i = 0; i< itemEvent.TextKey.Count; i++)
        {
            var textKey = itemEvent.TextKey[i];
            
            // TODO:文字送りスキップなど
            Debug.Log(LocalizeText.GetText(textKey));
            _itemText.text = LocalizeText.GetText(textKey);

            await WaitDecision();
        }
        
        await _itemTextCanvasGroup.DOFade(0f, _itemTextCVGDuration);
    }

    private async UniTask InAnimation()
    {
        Sequence mainSequence = DOTween.Sequence();
        Sequence viewAreaSequence = DOTween.Sequence();

        mainSequence.Join(_itemNameBG.rectTransform.DOAnchorPosY(0, _moveDuration).SetEase(Ease.InQuint));
        viewAreaSequence.Join(_viewAreaRoot.DOAnchorPosY(0, _moveDuration).SetEase(Ease.InQuint)).SetDelay(_moveDelayDuration);
        mainSequence.Join(viewAreaSequence);
        
        await mainSequence;
    }

    private async UniTask ExitAnimation()
    {
        Sequence mainSequence = DOTween.Sequence();
        Sequence itemAreaSequence = DOTween.Sequence();

        mainSequence.Join(_viewAreaRoot.DOAnchorPosY(_viewAreaRootPosition.y, _moveDuration).SetEase(Ease.InQuint));
        itemAreaSequence.Join(_itemNameBG.rectTransform.DOAnchorPosY(_itemNameBGPosition.y, _moveDuration).SetEase(Ease.InQuint)).SetDelay(_moveDelayDuration);
        mainSequence.Join(itemAreaSequence);
        
        await mainSequence;
    }

    bool isDecision = false;
    bool isPress = true;

    private async UniTask WaitDecision()
    {
        isDecision = false;
        isPress = true;

        CancelToken cancelToken = new CancelToken();

        InputManager.Instance.Decision.Subscribe(x =>
        {
            if (x == 0)
            {
                isPress = false;
            }

            if (isPress)
            {
                return;
            }

            if (x != 1)
            {
                return;
            }

            isDecision = true;
            isPress = true;
            cancelToken.Cancel();
        }).AddTo(cancelToken.GetToken());

        await UniTask.WaitUntil(() => isDecision);
        isDecision = false;
    }

    private void Reset()
    {
        _itemNameBGPosition = _itemNameBG.rectTransform.anchoredPosition;
        _viewAreaRootPosition = _viewAreaRoot.anchoredPosition;
        _itemTextCanvasGroup.alpha = 0;
        
        _itemName.text = "";
        _itemText.text = "";
    }
}