using Cysharp.Threading.Tasks;
using UnityEngine;

public class SonarViewTest : MonoBehaviour
{
    SonarConverter converter;
    // Start is called before the first frame update
    void Start()
    {
        converter=SonarConverter.Instance;
        viewTest();
    }

    // Update is called once per frame
    async void viewTest()
    {
        await UniTask.WaitForSeconds(1);
        converter.ViewSonar(SonarType.TutorialFirstDoor);

        await UniTask.WaitForSeconds(5);
        converter.HideSonar(SonarType.TutorialFirstDoor);
    }
}
