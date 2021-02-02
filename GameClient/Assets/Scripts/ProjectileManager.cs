using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public int m_ID;
    public GameObject m_ExplosionPrefab;

    public void Initialize(int iId)
    {
        m_ID = iId;
    }

    public void Explode(Vector3 iPosition)
    {
        transform.transform.position = iPosition;
        Instantiate(m_ExplosionPrefab, transform.position, Quaternion.identity);
        GameManager.s_Dic_Projectiles.Remove(m_ID);
        Destroy(gameObject);
    }
}
