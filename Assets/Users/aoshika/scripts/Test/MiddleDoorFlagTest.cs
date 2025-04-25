using UnityEngine;

public class MiddleDoorFlagTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialFlowManager.Instance.CheckMiddleDoorFlag();
        }
    }
}
