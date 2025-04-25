using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    public Vector3 LastFramePosition;
    public Vector3 Position;
    public Transform Head;
    public Camera Camera;
    public AudioListener Listener;
    [SerializeField] private Light _flashlight;
    [SerializeField] private Light _defaultLight;
    public Vector3 MoveDirection => (Position - LastFramePosition).normalized; 
    private void FixedUpdate()
    {
        LastFramePosition = Position;
        Position = transform.position;
    }

    public void UpdatePosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    public async UniTask Flashlight(bool active,float duration = 2.0f)
    {
        await DOVirtual.Float(0f, 1f, duration,
            (v) =>
            {
                var isActive = (v <= 0.9f);
                isActive ^= active;
                _flashlight.gameObject.SetActive(isActive);
            }).SetEase(Ease.OutBounce).ToUniTask();
    }

    public void LightSetActive(bool active)
    {
        _flashlight.enabled = active;
        _defaultLight.enabled = active;
    }
}