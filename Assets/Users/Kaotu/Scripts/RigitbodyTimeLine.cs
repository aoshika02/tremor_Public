using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigitbodyTimeLine : MonoBehaviour
{
    [SerializeField]
    private float m_power = 0.0f;
    [SerializeField]
    private Vector3 m_powerDir = Vector3.zero;
    [SerializeField]
    private Vector3 m_offset = Vector3.zero;

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.AddForceAtPosition(m_powerDir.normalized * m_power, transform.position + m_offset);
        }
    }
}
