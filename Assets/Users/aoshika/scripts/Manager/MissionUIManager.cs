using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionUIInfo
{
    public GameObject missions;
    public MissionUI MissionUI;
    public MissionUIHash MissionUIHash;
}

public class MissionUIManager : SingletonMonoBehaviour<MissionUIManager>
{
    public List<MissionUIInfo> indicatingMissionUI = new List<MissionUIInfo>();
    [SerializeField] private GameObject _missionUIObj;
    [SerializeField] private Transform _canvas;

    public MissionUIHash AddMission(string _text)
    {
        GameObject mission = Instantiate(_missionUIObj, _canvas);

        MissionUI MissionUI = mission.GetComponent<MissionUI>();
        MissionUI.AddMission(_text).Forget();


        var missionUIInfo = new MissionUIInfo
        {
            missions = mission,
            MissionUI = mission.GetComponent<MissionUI>(),
            MissionUIHash = new MissionUIHash()
        };
        indicatingMissionUI.Add(missionUIInfo);
        MissionUIPos();
        return missionUIInfo.MissionUIHash;
    }

    public async UniTask ClearMission(MissionUIHash missionUIHash)
    {
        MissionUIInfo uiInfo = null;
        for (int i = 0; i < indicatingMissionUI.Count; i++)
        {
            if (indicatingMissionUI[i].MissionUIHash == missionUIHash)
            {
                uiInfo = indicatingMissionUI[i];
                await uiInfo.MissionUI.ClearMission();
                indicatingMissionUI.Remove(uiInfo);
            }
        }

        MissionUIPos();
    }
    public void AllClearMission()
    {
        MissionUIInfo uiInfo = null;
        for (int i = 0; i < indicatingMissionUI.Count; i++)
        {
            uiInfo = indicatingMissionUI[i];
            uiInfo.MissionUI.AllClearMission();
            indicatingMissionUI.Remove(uiInfo);
        }
    }
    private void MissionUIPos()
    {
        GameObject mission;
        for (int i = 0; i < indicatingMissionUI.Count; i++)
        {
            mission = indicatingMissionUI[i].missions;
            var rect = mission.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector3(30, -100 * i - 45, 0);
        }
    }

    private TextMeshProUGUI GetTextMeshPro()
    {
        var textMeshPro = gameObject.AddComponent<TextMeshProUGUI>();

        return textMeshPro;
    }
}