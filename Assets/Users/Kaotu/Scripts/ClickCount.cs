using UnityEngine;

public class ClickCount : MonoBehaviour
{
    [SerializeField]
    private IpadCanvas ipadCanvas;
    
   public void Onclick()
    {
        ipadCanvas.Clickcount++;
    }

    public void OnEnter()
    {
        ipadCanvas.Entercount++;
    }
}
