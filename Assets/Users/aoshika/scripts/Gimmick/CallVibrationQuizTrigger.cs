using UnityEngine;

public class CallVibrationQuizTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            InGameFlow.Instance.QuizStartFlag();
        }
    }
}
