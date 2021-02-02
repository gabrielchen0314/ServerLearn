using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int m_ID;
    public float m_Health;
    public float m_MaxHealth = 100;

    public void Initialize(int iID)
    {
        m_ID = iID;
        m_Health = m_MaxHealth;
    }

    public void SetHealth(float iHealth)
    {
        m_Health = iHealth;

        if (m_Health <= 0f)
        {
            GameManager.s_Dic_Enemys.Remove(m_ID);
            Destroy(gameObject);
        }
    }
}
