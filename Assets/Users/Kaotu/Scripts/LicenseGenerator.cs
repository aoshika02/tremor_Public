using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class LicenseGenerator : SingletonMonoBehaviour<LicenseGenerator>
{
    [SerializeField] TextAsset _creditText;
    [SerializeField] TextAsset _licenseText;

    [SerializeField] private TextMeshProUGUI _outTextMeshProUGUI;

    private bool _isLicenseView = false;
    public bool IsView => _isLicenseView;

    [SerializeField] private GameObject _licensetext;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private float _scrollSpeed = 100f;

    private void Start()
    {
        string text = $"{_creditText.text}\n{_licenseText.text}";
        _outTextMeshProUGUI.text = text;

        _licensetext.SetActive(false);

        // ライセンスボタンの入力
        InputManager.Instance.License.Subscribe(x =>
        {
            if (AudioSetting.Instance?.IsView == true) { return; }
            if (TitleLoadMainGame.Instance?.IsLoad == true) { return; }
            if (x != 1)
            {
                return;
            }

            _licensetext.SetActive(!_isLicenseView);
            _isLicenseView = !_isLicenseView;
        }).AddTo(this);

        // ライセンスのスクロール
        InputManager.Instance.Move.Subscribe(x =>
        {
            if (!_isLicenseView)
            {
                return;
            }

            _scrollRect.velocity = new Vector2(0f, -x.y * _scrollSpeed);
        }).AddTo(this);
    }
}