using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabletUITest : MonoBehaviour
{
    TabletTaskUI tabletTaskUI;
    void Start()
    {
        tabletTaskUI=TabletTaskUI.Instance;
        tabletTaskUI.InGameTaskUI().Forget();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            tabletTaskUI.ClearTutorialFlag();
            tabletTaskUI.ResearchFlag();
            
            tabletTaskUI.IntoScoolFlag();
            tabletTaskUI.GetKeyFlag();
            tabletTaskUI.UseKeyFlag();
            tabletTaskUI.GetCursedItemFlag();

          //  tabletTaskUI.DiscoverEntranceGateFlag();
        }
    }
}


