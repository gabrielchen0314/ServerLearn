using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int m_ID;
    public string m_UserName;
    public float m_Health;
    public float m_MaxHealth;
    public MeshRenderer m_Model;

    public void Initialize(int iID,string iUserName)
    {
        m_ID = iID;
        m_UserName = iUserName;
        m_Health = m_MaxHealth;
    }

    public void SetHealth(float iHealth)
    {
        m_Health = iHealth;

        if(m_Health <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        m_Model.enabled = false;
    }

    public void Respawn()
    {
        m_Model.enabled = true;
        SetHealth(m_MaxHealth);
    }
}
