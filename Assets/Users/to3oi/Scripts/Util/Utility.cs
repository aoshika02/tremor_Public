public static class Utility
{
    public static bool IntToBool(int i) => i == 1;
    public static int BoolToInt(bool b) => b ? 1 : 0;

#if UNITY_EDITOR
    public static bool GetTutorialSkip() => Utility.IntToBool(UnityEngine.PlayerPrefs.GetInt("TutorialSkip",0));
    public static bool GetGameFlowSkip() => Utility.IntToBool(UnityEngine.PlayerPrefs.GetInt("GameFlowSkip",0));
    public static bool GetQuizSkip() => Utility.IntToBool(UnityEngine.PlayerPrefs.GetInt("QuizSkip",0));
    public static bool GetTabletEventSkip() => Utility.IntToBool(UnityEngine.PlayerPrefs.GetInt("TabletEventSkip",0));
#endif
    
    public static Cysharp.Threading.Tasks.UniTask PlayAsync(this UnityEngine.Playables.PlayableDirector self)
    {
        self.Play();
        return Cysharp.Threading.Tasks.UniTask.WaitWhile(() => self.state == UnityEngine.Playables.PlayState.Playing);
    }
}