using UnityEngine;

public class CallFirstWallAct : MonoBehaviour
{
    WallFall _wallFall;
    MapLocation _mapLocation;
    private void Start()
    {
        _wallFall=WallFall.Instance;
        _mapLocation = MapLocation.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _wallFall.ActFirstWallFall();
            _mapLocation.FirstWallAlpha(1);
        }
    }
}
