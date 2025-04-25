using UnityEngine;

public class FPSSetting : SingletonMonoBehaviour<FPSSetting>
{
    protected override void Awake()
    {
        if (CheckInstance())
        {
            Cursor.visible = false;
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        }
    }
}
