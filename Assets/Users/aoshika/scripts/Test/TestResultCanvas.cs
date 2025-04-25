using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestResultCanvas : MonoBehaviour
{
    ResultCanvasController resultCanvasController;
    private void Start()
    {
        resultCanvasController = ResultCanvasController.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            resultCanvasController.GameClearCanvas();
        }
    }
}
