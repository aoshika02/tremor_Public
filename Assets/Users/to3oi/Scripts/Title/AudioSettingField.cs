using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingField : MonoBehaviour
{
    [SerializeField] private RawImage _bg;
    [SerializeField] private TextMeshProUGUI _viewName;
    [SerializeField] private Slider _volumeSlider;

    [SerializeField] private GameObject _underLine;
    [SerializeField] private GameObject _cycleStartedImage;
    [SerializeField] private GameObject _cycleStartImage;

    AudioSetting.AudioSettingData _data;

    public bool IsSE => _data.IsSE;
    public string AudioName => _data.AudioName;

    public void Init(AudioSetting.AudioSettingData data)
    {
        _underLine.SetActive(false);
        _cycleStartedImage.SetActive(false);
        _cycleStartImage.SetActive(false);

        _data = data;
        _viewName.text = data.ViewName;

        // ãtïœä∑ÇﬂÇÒÇ«Ç¢ÇÃÇ≈Ç∆ÇËÇ†Ç¶Ç∏1Ç≈èâä˙âª
        _volumeSlider.value = GetVolume();
        _data.AudioMixerGroup.audioMixer.SetFloat(_data.AudioMixerGroup.name,
            Mathf.Clamp(Mathf.Log10(_volumeSlider.value) * 20f, -80f, 0f));
    }

    public void Select()
    {
        _bg.color = Color.gray;
        _underLine.SetActive(true);
        _cycleStartedImage.SetActive(false);
        _cycleStartImage.SetActive(true);
    }

    public void UnSelect()
    {
        _bg.color = Color.white;
        _underLine.SetActive(false);
        _cycleStartedImage.SetActive(false);
        _cycleStartImage.SetActive(false);
    }
    
    public void SetPlaySound(bool isPlay)
    {
        _cycleStartedImage.SetActive(isPlay);
        _cycleStartImage.SetActive(!isPlay);
    }


    public void MoveSlider(float value)
    {
        _volumeSlider.value += value;
        _data.AudioMixerGroup.audioMixer.SetFloat(_data.AudioMixerGroup.name,
            Mathf.Clamp(Mathf.Log10(_volumeSlider.value) * 20f, -80f, 0f));
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetFloat($"Volume_{_data.AudioMixerGroup.name}", _volumeSlider.value);
    }
    private float GetVolume()
    {
        return PlayerPrefs.GetFloat($"Volume_{_data.AudioMixerGroup.name}", 1);
    }
}