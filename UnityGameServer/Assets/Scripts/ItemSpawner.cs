using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static Dictionary<int, ItemSpawner> s_Dic_Spawners = new Dictionary<int, ItemSpawner>();
    private static int m_NextSpawnerID = 1;

    public int m_SpawnerID;
    public bool m_IsHasItem;

    private void Start()
    {
        m_IsHasItem = false;
        m_SpawnerID = m_NextSpawnerID;
        m_NextSpawnerID++;
        s_Dic_Spawners.Add(m_SpawnerID, this);

        StartCoroutine(SpaenItem());
    }

    private void OnTriggerEnter(Collider iCollider)
    {
        if (m_IsHasItem && iCollider.CompareTag("Player"))
        {
            Player _Player = iCollider.GetComponent<Player>();
            if(_Player.IsAttemptPickupItem())
            {
                ItemPickedUp(_Player.m_ID);
            }
        }
    }

    private IEnumerator SpaenItem()
    {
        yield return new WaitForSeconds(10f);

        m_IsHasItem = true;
        ServerSend.ItemSpawned(m_SpawnerID);
    }

    private void ItemPickedUp(int iPlayer)
    {
        m_IsHasItem = false;
        ServerSend.ItemPickedUp(m_SpawnerID, iPlayer);

        StartCoroutine(SpaenItem());
    }
}
