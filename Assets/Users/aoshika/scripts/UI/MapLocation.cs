using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MapLocation : SingletonMonoBehaviour<MapLocation>
{
    Player _player;
    [SerializeField] private GameObject _keyObj;
    [SerializeField] private GameObject _cursedObj;
    [SerializeField] private GameObject _goalObj;

    [SerializeField] private Image _currentPosition;
    [SerializeField] private Image _itemPosition;
    [SerializeField] private Image _keyPosition;
    [SerializeField] private Image _goalPosition;

    [SerializeField] private CanvasGroup _firstWall;
    [SerializeField] private CanvasGroup _entranceWall;
    [SerializeField] private CanvasGroup _classRoomWall;
    [SerializeField] private CanvasGroup _currentPositionGrop;
    [SerializeField] private CanvasGroup _itemPositionGrop;
    [SerializeField] private CanvasGroup _keyPositionGrop;
    [SerializeField] private CanvasGroup _goalPositionGrop;

    private void Start()
    {
        _player = Player.Instance;
        TabletPoint(_itemPosition,
            new Vector3(_cursedObj.transform.position.x, _cursedObj.transform.position.y, _cursedObj.transform.position.z));
        TabletPoint(_keyPosition,
            new Vector3(_keyObj.transform.position.x, _keyObj.transform.position.y, _keyObj.transform.position.z));
        TabletPoint(_goalPosition,
            new Vector3(_goalObj.transform.position.x, _goalObj.transform.position.y, _goalObj.transform.position.z));

        KeyPosAlpha();
        ItemPosAlpha();
        currentPosAlpha();
        FirstWallAlpha();
        EntranceWallAlpha();
        ClassRoomWallAlpha();
        GoalPosAlpha();
    }
    private void FixedUpdate()
    {
        TabletPoint(_currentPosition, _player.Position);
    }
    private Vector3 TabletPoint(Image pointImage, Vector3 objPoint)
    {
        var rect = pointImage.GetComponent<RectTransform>();
        //rect.anchoredPosition = new Vector3(objPoint.z * -13 + 867, objPoint.x * 13 - 1080, 0);TGSでの式
        rect.anchoredPosition = new Vector3(objPoint.z * 12.5f - 124.75f, objPoint.x * -12.5f + 139.75f, 0);
        return rect.anchoredPosition;
    }
    public void currentPosAlpha(float alpha = 0f)
    {
        _currentPositionGrop.alpha = alpha;
    }
    public void KeyPosAlpha(float alpha = 0f)
    {
        _keyPositionGrop.alpha = alpha;
    }
    public void ItemPosAlpha(float alpha = 0f)
    {
     _itemPositionGrop.alpha = alpha;
    }public void GoalPosAlpha(float alpha = 0f)
    {
        _goalPositionGrop.alpha = alpha;
    }
    public void FirstWallAlpha(float alpha = 0f)
    {
        _firstWall.alpha = alpha;
    }
    public void EntranceWallAlpha(float alpha = 0f)
    {
        _entranceWall.alpha = alpha;
    }
    public void ClassRoomWallAlpha(float alpha = 0f)
    {
        _classRoomWall.alpha = alpha;
    }
}
