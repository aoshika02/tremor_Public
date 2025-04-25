using UnityEngine;

public class CallClassroomSideWallAct : MonoBehaviour
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
            _wallFall.ActClassroomSideWallFall();
        }
    }
}
