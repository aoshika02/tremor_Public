using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class IpadCanvas : MonoBehaviour
{
    public int Clickcount = 0;
    public int Entercount = 0;
    private bool ischask = false;

    [SerializeField]
    private ClickCount clickCount;
    [SerializeField]
    private TextMeshProUGUI textMeshPro;
    [SerializeField]
    private RawImage rawImage;
    [SerializeField]
    private GameObject triangle;
    [SerializeField]
    private GameObject triangle2;
    // Update is called once per frame
    private void Start()
    {
        InputManager.Instance.Decision.Subscribe(x =>
        {
            if (x == 1f)
            {
                clickCount.Onclick();
            }

            if (x == 0.0f) return;
        }).AddTo(this);
        textMeshPro.text = "�`���[�g���A��TEXT";
    }
     void Update()
     {
         if (Clickcount != 1)
             switch (Clickcount)
             {
                 case 2:
                     triangle.SetActive(false);
                     triangle2.SetActive(true);
                     textMeshPro.text = "�J��1";
                     rawImage.color = Color.white;
                     break;
                 case 3:
                     textMeshPro.text = "�J��2";
                     rawImage.color = Color.red;
                     break;
                 case 4:
                     textMeshPro.text = "�J��3";
                     rawImage.color = Color.green;
                     break;
                 case 5:
                     textMeshPro.text = "�J��4";
                     rawImage.color = Color.blue;
                     break;
             }
         else
         {
             textMeshPro.text = "�`���[�g���A��2TEXT";
             ischask = true;
         }
     }
     }
   
