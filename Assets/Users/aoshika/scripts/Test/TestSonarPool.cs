using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestSonarPool : MonoBehaviour
{
    [SerializeField] private GameObject _test;

    private List<(string, GameObject)> list = new List<(string, GameObject)>();
    // Start is called before the first frame update
    void Start()
    {
        //SonarObject sonarObject= SonarObjectPool.Instance.GetSonarObject();
        //sonarObject.Initialize(Color.red, _test.transform.position);
    }
    public void SetSonar(string key)
    {
        SonarObject sonarObject = SonarObjectPool.Instance.GetSonarObject();
        sonarObject.Initialize(Color.red,list.FirstOrDefault(x => x.Item1 == key).Item2.transform.position);
    }
}