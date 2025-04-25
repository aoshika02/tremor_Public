using UnityEngine;
public class EnemyDebuffs : MonoBehaviour
{
    PlayerSAN _playerSAN;
    private void Start()
    {
        _playerSAN = PlayerSAN.Instance;
        _playerSAN?.AddEnemyTransform(transform);
    }
    private void OnEnable()
    {
        _playerSAN?.AddEnemyTransform(transform);
    }
    private void OnDisable()
    {
        _playerSAN?.RemoveEnemyTransform(transform);
    }
    private void OnDestroy()
    {
        _playerSAN?.RemoveEnemyTransform(transform);
    }
}
