using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetDocumentTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            TutorialFlowManager.Instance.GetDocumentFlag();
        }
    }
}
