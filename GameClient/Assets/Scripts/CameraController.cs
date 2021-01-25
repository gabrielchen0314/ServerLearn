using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerManager m_Player;
    public float m_Sensitivity = 100f;
    public float m_ClampAngle = 85f;

    private float m_VerticalRotation;
    private float m_HorizontalRotation;

    private void Start()
    {
        m_VerticalRotation = transform.localEulerAngles.x;
        m_HorizontalRotation = m_Player.transform.eulerAngles.y;
    }

    private void Update()
    {
        Look();
        Debug.DrawRay(transform.position, transform.forward * 2, Color.red);
    }

    private void Look()
    {
        float _MouseVertical = -Input.GetAxis("Mouse Y");
        float _MouseHorizontal = Input.GetAxis("Mouse X");

        m_VerticalRotation += _MouseVertical * m_Sensitivity * Time.deltaTime;
        m_HorizontalRotation += _MouseHorizontal * m_Sensitivity * Time.deltaTime;

        m_VerticalRotation = Mathf.Clamp(m_VerticalRotation, -m_ClampAngle, m_ClampAngle);

        transform.localRotation = Quaternion.Euler(m_VerticalRotation, 0f, 0f);
        m_Player.transform.rotation = Quaternion.Euler(0f, m_HorizontalRotation, 0f);
    }
}
