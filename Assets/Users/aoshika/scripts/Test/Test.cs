using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class Test : MonoBehaviour
{
    MissionUIManager missionUIManager;
    async UniTask Start()
    {
        var _1 = MissionUIManager.Instance.AddMission("AB");
        await UniTask.Delay(TimeSpan.FromSeconds(1)); 
        var _2 = MissionUIManager.Instance.AddMission("CD");
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        var _3 = MissionUIManager.Instance.AddMission("EF");
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        var _4 = MissionUIManager.Instance.AddMission("GH");
        await UniTask.Delay(TimeSpan.FromSeconds(1)); 
        var _5 = MissionUIManager.Instance.AddMission("fhgjklfghj");
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        await MissionUIManager.Instance.ClearMission(_1);
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        await MissionUIManager.Instance.ClearMission(_2);
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        await MissionUIManager.Instance.ClearMission(_3);
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        await MissionUIManager.Instance.ClearMission(_4);
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        await MissionUIManager.Instance.ClearMission(_5);

    }

}


