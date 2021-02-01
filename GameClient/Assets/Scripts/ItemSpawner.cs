using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public int m_SpawnerID;
    public bool m_IsHsaItem;
    public MeshRenderer m_ItemModel;

    public float m_ItemRotationSpeed = 50f;
    public float m_ItemBobSpeed = 2f;

    private Vector3 m_BasePosition;

    private void Update()
    {
        if (m_IsHsaItem)
        {
            transform.Rotate(Vector3.up, m_ItemRotationSpeed * Time.deltaTime, Space.World);
            transform.position = m_BasePosition + new Vector3(0f, 0.25f * Mathf.Sin(Time.time * m_ItemBobSpeed),0f);
        }   
    }

    public void Initialize(int iSpawnerID,bool iIsHasItem)
    {
        m_SpawnerID = iSpawnerID;
        m_IsHsaItem = iIsHasItem;
        m_ItemModel.enabled = iIsHasItem;

        m_BasePosition = transform.position;
    }

    public void ItemSpawned()
    {
        m_IsHsaItem = true;
        m_ItemModel.enabled = true;
    }

    public void ItemPickedUp()
    {
        m_IsHsaItem = false;
        m_ItemModel.enabled = false;
    }
}
