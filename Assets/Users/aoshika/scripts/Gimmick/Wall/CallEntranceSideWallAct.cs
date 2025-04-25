using UnityEngine;

public class CallEntranceSideWallAct : MonoBehaviour
{
    WallFall _wallFall;
    private void Start()
    {
        _wallFall=WallFall.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _wallFall.ActEntranceSideWallFall();
        }
    }
}
