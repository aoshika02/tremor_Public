using UnityEngine;
public class GetNewsPaperTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialFlowManager.Instance.GetNewsPaperFlag();
        }
    }
}
