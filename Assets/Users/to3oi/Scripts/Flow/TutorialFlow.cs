using UnityEngine;
using Cysharp.Threading.Tasks;

public class TutorialFlow : SingletonMonoBehaviour<TutorialFlow>
{
    private Player _player;
    [SerializeField] private Transform _playerStartPosition;

    private void Start()
    {
        _player = Player.Instance;
        _player.transform.position = _playerStartPosition.position;
        StartFlow().Forget();
    }

    /// <summary>
    /// スタートからチュートリアルまでのフローを管理する
    /// </summary>
    private async UniTask StartFlow()
    {
    }

    public void TabletEvent()
    {
        TutorialTablet.Instance.StartTutorialTablet();
    }
}