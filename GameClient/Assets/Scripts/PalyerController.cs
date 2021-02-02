using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalyerController : MonoBehaviour
{
    public Transform m_CameraTransform;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            ClientSend.PlayerShoot(m_CameraTransform.forward);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ClientSend.PlayerThrowItem(m_CameraTransform.forward);
        }
    }
    private void FixedUpdate()
    {
        SendInputToServer();
    }

    private void SendInputToServer()
    {
        bool[] _inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Space),
        };

        ClientSend.PlayerMovement(_inputs);
    }
    
}
