using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;

public class GameClearManager : SingletonMonoBehaviour<GameClearManager>
{
    [SerializeField]
    private PlayableDirector _gameclear;
    //[SerializeField]
   // private GameObject _image;
    /*[SerializeField]
    private TextMeshProUGUI _textend;
    [SerializeField]
    private TextMeshProUGUI _textstart;
    [SerializeField]
    private TextMeshProUGUI _texttitle;
    public static string title;
    public static string gameob;
    public static string textend;
    */
    private bool _isClick = false;
    private bool _isFirst = true;

    private async void Start()
    {
       // _textend.text = gameob + "На";
        //_textstart.text = textend + "На";
       // _texttitle.text = title + "На";
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        GameClear().Forget();
    }

    private async UniTask GameClear()
    {
        PlayerPrefs.SetInt("ClearCount",PlayerPrefs.GetInt("ClearCount" , 0) + 1);
        PlayerPrefs.Save();
        
       
        
        await _gameclear.PlayAsync();
        //_image.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

        /*
                InputManager.Instance.Decision.Subscribe(x =>
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
        await UniTask.Delay(TimeSpan.FromSeconds(3.5f));
        
        await LoadThankYouForPlaying();
    }

    private async UniTask LoadThankYouForPlaying()
    {
        await FadeInOut.FadeIn();
        await SceneLoadManager.Instance.AddLoadScene(SceneType.ThankYouForPlaying);
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        SceneLoadManager.Instance.LoadedSceneActive(SceneType.ThankYouForPlaying, isOutFade:true).Forget();
    }
}