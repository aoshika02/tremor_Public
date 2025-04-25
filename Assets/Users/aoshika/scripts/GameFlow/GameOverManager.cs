using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using DG.Tweening;

public class GameOverManager : SingletonMonoBehaviour<GameOverManager>
{
    private bool _isClick = false;
    private bool _isFirst = true;
    [SerializeField]private Material _material;
    private async void Start()
    {
        //初期化
        _material.SetFloat("_Dissolve", 0);
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        GameOver().Forget();
    }
    private async UniTask GameOver() 
    {
        PlayerPrefs.SetInt("GameOverCount", PlayerPrefs.GetInt("GameOverCount", 0) + 1);
        PlayerPrefs.Save();

        await GameOverImgAlpha();

        /*        InputManager.Instance.Decision.Subscribe(x =>
                {
                    if(x != 1f) {return;}
                    if(!_isFirst) {return;}
                    _isFirst = false;
                    _isClick = true;
                }).AddTo(this);

                _isClick = false;
                await UniTask.WaitUntil(() => _isClick);
        */

        await UniTask.Delay(TimeSpan.FromSeconds(1f));

        await LoadThankYouForPlaying();
    }

    private async UniTask LoadThankYouForPlaying()
    {
        await FadeInOut.FadeIn();
        await SceneLoadManager.Instance.AddLoadScene(SceneType.ThankYouForPlaying);
        SceneLoadManager.Instance.LoadedSceneActive(SceneType.ThankYouForPlaying,isOutFade:true).Forget();
    }

    private async UniTask GameOverImgAlpha()
    {
        SoundManager.Instance.PlaySE(SEType.GameOver);
        await DOVirtual.Float(0.1f, 0.7f, 4, f =>
        {
            _material.SetFloat("_Dissolve",f);
        }).ToUniTask();
    }
}
