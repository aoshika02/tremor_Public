using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    //MissionUIManagerを介してhashの結びつけを行う

    //textの更新

    //特定の動作をしたときチェックボックスを変化
    //チェックボックスが変化したとき消える処理を呼び出す
    //消える処理：
    //
    [SerializeField] private TextMeshProUGUI _missionText;
    [SerializeField] private CanvasGroup _missionTextGroup;

    [SerializeField] private CanvasGroup _greenCheckBox;
    [SerializeField] private CanvasGroup _grayCheckBox;
    [SerializeField] private CanvasGroup _checkImage;
    public void Awake() 
    {
        _missionText.text = "";
        _missionTextGroup.alpha = 0;
        _greenCheckBox.alpha = 0;
        _grayCheckBox.alpha = 0;
        _checkImage.alpha = 0;
    }
    public async UniTask AddMission(string text)
    {
        _grayCheckBox.alpha = 1;
        _missionText.text = text;
        await DOVirtual.Float(0 ,1, 0.25f, f =>
        {
            _missionTextGroup.alpha = f;
        }).ToUniTask();

    }
    public async UniTask ClearMission() 
    {
        await DOVirtual.Float(0, 1, 1f, f =>
        {
            _greenCheckBox.alpha = f;
            _checkImage.alpha = f;
        }).ToUniTask();
        await DOVirtual.Float(1, 0, 0.25f, f =>
        {
            _missionTextGroup.alpha = f;
        }).ToUniTask();
    }
    public void AllClearMission()
    {
        _missionTextGroup.alpha = 0;
    }
}
