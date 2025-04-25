using TMPro;
using UnityEngine;

public class Debug_ViewDistance : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _distanceText;

    private Player _player;
    private void Start()
    {
        _player = Player.Instance;
    }

    void FixedUpdate()
    {
        _distanceText.text = "Distance:" + Vector3.Distance(_player.Position, transform.position).ToString("F2");
    }
}
