using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SceneLoadManager;

public class Load : MonoBehaviour
{
    //SceneLoadManager sceneLoadManager;
    SoundManager soundManager;
    // Start is called before the first frame update
    void Start()
    {
        //sceneLoadManager = GetComponent<SceneLoadManager>();
        //sceneLoadManager.LoadScene(SceneType.SampleMap).Forget();
        soundManager = GetComponent<SoundManager>();
        soundManager.PlaySE((SEType)9001);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
