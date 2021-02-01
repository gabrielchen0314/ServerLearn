﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager s_instance;

    public static Dictionary<int, PlayerManager> s_Dic_Players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, ItemSpawner> s_Dic_ItemSpawners = new Dictionary<int, ItemSpawner>();

    public GameObject m_localPlayerPrefab;
    public GameObject m_PlayerPrefab;
    public GameObject m_ItemSpawnerPrefab;

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
}
