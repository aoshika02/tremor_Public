using System;
using UniRx;
using UnityEngine;

public class PlayerItemHold : SingletonMonoBehaviour<PlayerItemHold>
{
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _playerCatch;
    [SerializeField] private GameObject _tablet;
    private bool _isCall = false;
    private Transform _holdTransform;
    public bool IsHold{ get; private set; } = false;
    PlayerMove _playerMove;
   
    /// <summary>
    /// アイテムを所持する処理
    /// </summary>
    /// <param name="itemTransform">該当アイテム</param>
    public void PlayerGetItem(Transform itemTransform)
    {
        if (_isCall) return;
        if (_holdTransform!=null) return;
        _isCall = true;
        IsHold = true;
        _holdTransform = itemTransform;
        _playerMove = PlayerMove.Instance;
        _playerMove.InvertMoveLockFlag();
        //TODO:タブレットをしまうモーション
        _tablet.SetActive(false);
        itemTransform.parent = _player;
        itemTransform.position = _playerCatch.position;
        _playerMove.InvertMoveLockFlag();
        _isCall = false;
    }
    /// <summary>
    /// アイテムを手放す処理
    /// </summary>
    /// <param name="itemTransform">該当アイテム</param>
    /// <param name="originalParent">該当アイテムの元の親</param>
    /// <param name="originalTransform">該当アイテムの元のTransform</param>
    public void PlayerReleaseItem(Transform itemTransform, Transform originalParent, Transform originalTransform)
    {
        if (_isCall) return;
        if (_holdTransform!= itemTransform) return;
        _isCall = true;
        IsHold = false;
        _holdTransform = null;
        _playerMove = PlayerMove.Instance;
        _playerMove.InvertMoveLockFlag();
        if (itemTransform.parent == _player)
        {
            itemTransform.parent = (originalParent == null) ? null : originalParent;
        }
        itemTransform.position = (originalTransform != null) ? originalTransform.position : _player.position;
        //TODO:タブレットを出すモーション
        _tablet.SetActive(true);
        _playerMove.InvertMoveLockFlag();
        _isCall = false;
    }
}
