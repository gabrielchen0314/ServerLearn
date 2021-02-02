using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager s_Instance;

    public GameObject m_PlayerPrefab;
    public GameObject m_EnemyPrefab;
    public GameObject m_ProjectilePrefab;
    private void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = this;
        }
        else if (s_Instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        //QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Server.Start(50, 26950);
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer()
    {
        return Instantiate(m_PlayerPrefab, new Vector3(0f,0.5f,0f), Quaternion.identity).GetComponent<Player>();
    }

    public void InstantiateEnemy(Vector3 iPosition)
    {
        Instantiate(m_EnemyPrefab, iPosition, Quaternion.identity);
    }

    public Projectile InstantiateProjectile(Transform iShootOrigin)
    {
        return Instantiate(m_ProjectilePrefab, iShootOrigin.position + iShootOrigin.forward * 0.7f, Quaternion.identity).GetComponent<Projectile>();
    }
}
