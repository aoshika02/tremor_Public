using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCursedItemFlag : MonoBehaviour
{
    InGameFlow inGameFlow;
    private void Start()
    {
        inGameFlow = InGameFlow.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            inGameFlow.CursedItemFlag();
        }
    }
}
