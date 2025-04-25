using UnityEngine;

public class TutorialTransformTest : MonoBehaviour
{
    TutorialTablet tutorialTablet;
    private void Start()
    {
        Invoke("TestAwake", 2);
    }
    private void TestAwake() 
    {
        tutorialTablet = TutorialTablet.Instance;
        tutorialTablet.StartTutorialTablet();
    }
   
}
