using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class SonarConverter : SingletonMonoBehaviour<SonarConverter>
{
    [SerializeField] private List<SonarPoint> _sonarKeyValues = new List<SonarPoint>();
    private List<(SonarType SonarType, SonarObject SonarObject)> _viewSonarObjects = new List<(SonarType, SonarObject)>();
    [Serializable]
    public class SonarPoint
    {
        public SonarType SonarType;
        public Color Color;
        public Transform ObjTransForm;
    }
    /// <summary>
    /// EnumからVectorを取得する
    /// </summary>
    /// <param name="objType"></param>
    /// <returns></returns>

    private SonarPoint GetSonarPoint(SonarType objType)
    {
        var t = _sonarKeyValues.FirstOrDefault(x => x.SonarType == objType);
        if (t == null)
        {
            t = new SonarPoint()
            {
                SonarType = objType,
                Color = Color.white,
                ObjTransForm = new GameObject().transform
            };
        }
        return t;
    }
    public void ViewSonar(SonarType sonarType)
    {
        SonarObject sonarObject = SonarObjectPool.Instance.GetSonarObject();
        SonarPoint sonarPointClass = GetSonarPoint(sonarType);
        sonarObject.Initialize(sonarPointClass.Color, sonarPointClass.ObjTransForm.position);
        _viewSonarObjects.Add((sonarType, sonarObject));
    }
    public void HideSonar(SonarType sonarType)
    {
        var t = _viewSonarObjects.FirstOrDefault(x => x.SonarType == sonarType);

        if (t.SonarObject == null) return;

        SonarObjectPool.Instance.ReleaseSonarObject(t.SonarObject);
        _viewSonarObjects.Remove(t);
    }
}
public enum SonarType
{
    None = 0,
    //Tutorial
    TutorialFirstDoor = 1,
    TutorialBlackoard = 2,
    MiddleDoor = 3,
    //InGameFlow
    ClassRoom2 = 4,
    ClassRoom3 = 5,
    MusicRoom = 6,
    PreparationRoom = 7,
    Hole = 8
}
