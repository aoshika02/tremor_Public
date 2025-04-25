using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System;

public class Battery : MonoBehaviour
{
    private bool isbool = false;
    public Animator animator;
    [SerializeField]
    private GameObject rawimage;
    [SerializeField]
    private GameObject[] battery;
    [SerializeField]
    private Image[] batteryimage;
    async void Start()
    {
        
        animator = GetComponent<Animator>();
       
        InGameFlow.Instance.TimeObservable.Subscribe(async x =>
        {   
            if (InGameFlow.Instance.TimeLimit / 5 * 4 >= x)
            {
                battery[4].SetActive(false);

                if (InGameFlow.Instance.TimeLimit / 5 * 3 >= x)
                {
                    battery[3].SetActive(false);
                    for(int i = 0; i < 4; i++)
                    {
                        batteryimage[i].color = new Color(255, 69, 0);
                    }

                    if (InGameFlow.Instance.TimeLimit / 5 * 2 >= x)
                    {
                        battery[2].SetActive(false);

                        if (InGameFlow.Instance.TimeLimit / 5 * 1 >= x)
                        {
                            battery[1].SetActive(false);
                            for (int i = 0;i < 4;i++)
                            {
                                batteryimage[i].color = Color.red;
                            }
                            if(InGameFlow.Instance.TimeLimit /5 * 0.5 >= x)
                            {
                                battery[0].gameObject.GetComponent<Animator>().SetBool("isbool", true);


                                if (InGameFlow.Instance.TimeLimit / 5 * 0 >= x)
                                {
                                    battery[0].SetActive(false);
                                    await UniTask.Delay(TimeSpan.FromSeconds(3));
                                    rawimage.SetActive(true);
                                }
                            }

                      
                        }
                    }
                }
            }
       
            if (x == 0) return;
            
        }).AddTo(this);
        


        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
