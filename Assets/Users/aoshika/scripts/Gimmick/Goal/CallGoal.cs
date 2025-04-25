using UnityEngine;

public class CallGoal : MonoBehaviour
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
            inGameFlow.GoalFlag();
        }
    }
}
