using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager s_instance;

    public static Dictionary<int, PlayerManager> s_Dic_Players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, ItemSpawner> s_Dic_ItemSpawners = new Dictionary<int, ItemSpawner>();
    public static Dictionary<int, ProjectileManager> s_Dic_Projectiles = new Dictionary<int, ProjectileManager>();
    public static Dictionary<int, EnemyManager> s_Dic_Enemys = new Dictionary<int, EnemyManager>();

    public GameObject m_localPlayerPrefab;
    public GameObject m_PlayerPrefab;
    public GameObject m_ItemSpawnerPrefab;
    public GameObject m_ProjectilePrefab;
    public GameObject m_EnemyPrefab;

    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
        }
        else if (s_instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void SpawnPlayer(int iID,string iUserName, Vector3 iPosition, Quaternion iRotation)
    {
        GameObject _player;
        if(iID == Client.s_instance.m_MyID)
        {
            _player = Instantiate(m_localPlayerPrefab, iPosition, iRotation);
        }
        else
        {
            _player = Instantiate(m_PlayerPrefab, iPosition, iRotation);
        }

        _player.GetComponent<PlayerManager>().Initialize(iID, iUserName);
        s_Dic_Players.Add(iID, _player.GetComponent<PlayerManager>());
    }

    public void CreateItemSpawner(int iSpawnerID, Vector3 iPosition,bool iIsHasItem)
    {
        GameObject _Spawner = Instantiate(m_ItemSpawnerPrefab, iPosition, m_ItemSpawnerPrefab.transform.rotation);
        _Spawner.GetComponent<ItemSpawner>().Initialize(iSpawnerID, iIsHasItem);
        s_Dic_ItemSpawners.Add(iSpawnerID,_Spawner.GetComponent<ItemSpawner>());
    }

    public void SpawnProjectile(int iId,Vector3 iPosition)
    {
        GameObject _Projectile = Instantiate(m_ProjectilePrefab, iPosition, Quaternion.identity);
        _Projectile.GetComponent<ProjectileManager>().Initialize(iId);
        s_Dic_Projectiles.Add(iId,_Projectile.GetComponent<ProjectileManager>());
    }

    public void SpawnEnemy(int iID, Vector3 iPosition)
    {
        GameObject _enemy = Instantiate(m_EnemyPrefab, iPosition, Quaternion.identity);
        _enemy.GetComponent<EnemyManager>().Initialize(iID);
        s_Dic_Enemys.Add(iID,_enemy.GetComponent<EnemyManager>());
    }
}
