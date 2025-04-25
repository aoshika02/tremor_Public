using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IkSet : MonoBehaviour
{
    [SerializeField]
    private GameObject _ik;

    public void IK()
    {
        _ik.SetActive(true);
    }
}
