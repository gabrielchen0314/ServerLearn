using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int m_ID;
    public string m_UserName;
    public CharacterController m_ChatacterController;
    public Transform m_ShootOrigin;
    public float m_fGravity = -9.81f;
    public float m_fMoveSpeed = 5f;
    public float m_fJumpSpeed = 5f;
    public float m_Health;
    public float m_MaxHealth = 100f;

    private bool[] m_Ay_Inputs;
    private float m_fyVelocity = 0;

    private void Start()
    {
        m_fGravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        m_fMoveSpeed *= Time.fixedDeltaTime;
        m_fJumpSpeed *= Time.fixedDeltaTime;
    }
    public void Initalize(int iID, string iUserName)
    {
        m_ID = iID;
        m_UserName = iUserName;
        m_Health = m_MaxHealth;

        m_Ay_Inputs = new bool[5];
    }

    public void FixedUpdate()
    {
        if(m_Health <= 0f)
        {
            return;
        }
        Vector2 _InputDirection = Vector2.zero;
        if (m_Ay_Inputs[0])
        {
            _InputDirection.y += 1;
        }
        if (m_Ay_Inputs[1])
        {
            _InputDirection.y -= 1;
        }
        if (m_Ay_Inputs[2])
        {
            _InputDirection.x -= 1;
        }
        if (m_Ay_Inputs[3])
        {
            _InputDirection.x += 1;
        }

        Move(_InputDirection);
    }

    private void Move(Vector2 iInputDirection)
    {
        Vector3 _MoveDirection = transform.right * iInputDirection.x + transform.forward * iInputDirection.y;
        _MoveDirection *= m_fMoveSpeed;

        if (m_ChatacterController.isGrounded)
        {
            m_fyVelocity = 0f;
            if(m_Ay_Inputs[4])
            {
                m_fyVelocity = m_fJumpSpeed;
            }
        }

        m_fyVelocity += m_fGravity;

        _MoveDirection.y = m_fyVelocity;
        m_ChatacterController.Move(_MoveDirection);

        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    public void SetInput(bool[] iInputs, Quaternion iRotation)
    {
        m_Ay_Inputs = iInputs;
        transform.rotation = iRotation;
    }

    public void Shoot(Vector3 iViewDirection)
    {
        if(Physics.Raycast(m_ShootOrigin.position,iViewDirection, out RaycastHit _hit, 25f))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                _hit.collider.GetComponent<Player>().TakeDamage(50f);
            }
        }
    }

    public void TakeDamage(float iDamage)
    {
        if(m_Health <= 0f)
        {
            
            return;
        }

        m_Health -= iDamage;

        if (m_Health <= 0f)
        {
            m_Health = 0;
            m_ChatacterController.enabled = false;
            transform.position = new Vector3(0f, 25f, 0f);
            ServerSend.PlayerPosition(this);
            StartCoroutine(Respawn());
        }

         ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        m_Health = m_MaxHealth;
        m_ChatacterController.enabled = true;
        ServerSend.PlayerRespawned(this);
    }
}
