using System.Collections.Generic;
using UnityEngine;
public class SonarObjectPool : SingletonMonoBehaviour<SonarObjectPool>
{
    [SerializeField] private Transform _parent;
    [SerializeField]private GameObject _sonarObjectPrefab;
    private List<SonarObject> _sonarObjects=new List<SonarObject>();
    private void Start()
    {
        for(int i = 0; i < 5; i++)
        {
            CreateSonarObject();
        }
    }
    public SonarObject GetSonarObject() 
    {
        for (int i=0;i< _sonarObjects.Count; i++) 
        {
            if (_sonarObjects[i].IsUse == false)
            {
                _sonarObjects[i].IsUse = true;
                return _sonarObjects[i];
            }
        }
        SonarObject sonarobj = CreateSonarObject();
        sonarobj.IsUse = true;
        return sonarobj;
    }
    public void ReleaseSonarObject(SonarObject sonarObject) 
    {
        sonarObject.IsUse = false;
        sonarObject.Deactivate();
    }
    private SonarObject CreateSonarObject() 
    {
        GameObject obj= Instantiate(_sonarObjectPrefab);
        obj.transform.SetParent(_parent);
        SonarObject sonarObj = obj.GetComponent<SonarObject>();
        sonarObj.Deactivate();
        _sonarObjects.Add(sonarObj);
        return sonarObj;
    }
}

