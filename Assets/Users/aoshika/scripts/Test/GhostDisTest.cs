using UniRx;
using UnityEngine;

public class GhostDisTest : MonoBehaviour
{
    PlayerSAN playerSAN;
    PlayerStatus playerStatus;
    void Start()
    {
        playerSAN=PlayerSAN.Instance;
        playerStatus = PlayerStatus.Instance;
        playerStatus.SanObservable.Subscribe(San =>
        {
            Debug.Log("SAN:"+San);
        });
    }
    void Update()
    {
        //playerSAN.UpdatePlayerSAN(transform);
    }
}
