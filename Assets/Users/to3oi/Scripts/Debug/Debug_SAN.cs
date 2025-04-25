using UnityEngine;

public class Debug_SAN : MonoBehaviour
{
    [SerializeField] private int _sanValue = 100;
    PlayerStatus _playerStatus;

    private void Start()
    {
        _playerStatus = PlayerStatus.Instance;
    }

    private void OnGUI()
    {
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 30;
        GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(400), GUILayout.Height(100) };

        if (GUILayout.Button("SetSANValue", buttonStyle, options))
        {
            if (_playerStatus.San > _sanValue)
            {
                _playerStatus.SubSAN(_playerStatus.San - _sanValue);
            }
            else if (_sanValue > _playerStatus.San)
            {
                _playerStatus.AddSAN(_sanValue - _playerStatus.San );
            }
        }
    }
}