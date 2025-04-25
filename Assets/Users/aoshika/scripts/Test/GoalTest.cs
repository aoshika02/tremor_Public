using UnityEngine;

public class GoalTest : MonoBehaviour
{
    InGameFlow inGameFlow;
    private void Start()
    {
        inGameFlow = InGameFlow.Instance;
        inGameFlow.StartInGame();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            inGameFlow.FirstWallFallFlag();
            inGameFlow.SecondWallFallFlag();
            inGameFlow.CursedItemFlag();
        }
    }
}
