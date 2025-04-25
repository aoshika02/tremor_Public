using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ThankYouForPlayingManager : SingletonMonoBehaviour<ThankYouForPlayingManager>
{
    bool _isAction = false;
    [SerializeField] private Image _fillImage;
    void Start()
    {
        // ‰Šú‰»
        _fillImage.fillAmount = 0;

        InputManager.Instance.ThankYouForPlayingProgress.Subscribe(async x =>
        {
            if(_isAction) {return;}
            // ’·‰Ÿ‚µ‚Ìi’»‚ğ•\¦‚·‚é
            _fillImage.fillAmount = x;
            
            if(x != 1) { return; }
            _isAction = true;
            // ”O‚Ìˆ×’¼Ú‘ã“ü
            _fillImage.fillAmount = 1;

            await FadeInOut.FadeIn();
            await SceneLoadManager.Instance.AddLoadScene(SceneType.Title);
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            SceneLoadManager.Instance.LoadedSceneActive(SceneType.Title, isOutFade: true).Forget();
        }).AddTo(this);
    }
}
