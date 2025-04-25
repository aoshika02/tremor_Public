using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System;

public class StaminaUI : SingletonMonoBehaviour<StaminaUI>
{
    PlayerStatus playerStatus;
    PlayerMove playerMove;
    [SerializeField] private float _playerStamina;
    [SerializeField] private float _maxPlayerStamina = 100;
    [SerializeField] private Slider _slider;

    [SerializeField] private Image _staminaBarImage;

    [SerializeField]private List<Colors> _staminaBarColors;
    [Serializable]
    private class Colors
    {
        [SerializeField]public Color StaminaBarColor;
    }
    void Start()
    {
        playerMove = PlayerMove.Instance;

        playerStatus = PlayerStatus.Instance;
        playerStatus.StaminaObservable.Subscribe(Stamina =>
        {
            _playerStamina = Stamina;
            _slider.value = Stamina / _maxPlayerStamina;
            StaminaBarColor(playerMove._dashStan);
        });

    }
   
    private void StaminaBarColor(bool _dash)
    {
        if (_dash)
        {
            _staminaBarImage.color = _staminaBarColors[4].StaminaBarColor;
        }
        else
        {
            if (_slider.value > 0.75f) 
            {
                _staminaBarImage.color = _staminaBarColors[0].StaminaBarColor;
            }
            else if (_slider.value > 0.5f) 
            {
                _staminaBarImage.color = _staminaBarColors[1].StaminaBarColor;
            }
         else if (_slider.value > 0.25f) 
            {
                _staminaBarImage.color = _staminaBarColors[2].StaminaBarColor;
            }
         else 
            {
                _staminaBarImage.color = _staminaBarColors[3].StaminaBarColor;
            }
        }
    }
}
