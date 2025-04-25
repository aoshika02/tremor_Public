using UnityEngine;

public class SecondDoorTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            TutorialFlowManager.Instance.OpenSecondDoorFlag();
        }
    }
}
