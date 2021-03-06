﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.s_instance.m_MyID = _myId;
        ClientSend.WelcomeReceived();

        Client.s_instance.m_UDP.Connect(((IPEndPoint)Client.s_instance.m_TCP.m_Socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet iPacket)
    {
        int _ID = iPacket.ReadInt();
        string _UserName = iPacket.ReadString();
        Vector3 _Postiton = iPacket.ReadVector3();
        Quaternion _Rotation = iPacket.ReadQuaternion();

        GameManager.s_instance.SpawnPlayer(_ID, _UserName, _Postiton, _Rotation);
    }

    public static void PlayerPosition(Packet iPacket)
    {
        int _ID = iPacket.ReadInt();
        Vector3 _Position = iPacket.ReadVector3();

        if (GameManager.s_Dic_Players.TryGetValue(_ID, out PlayerManager _player))
        {
            _player.transform.position = _Position;
        }
    }
    public static void PlayerRotation(Packet iPacket)
    {
        int _ID = iPacket.ReadInt();
        Quaternion _Rotation = iPacket.ReadQuaternion();

        if (GameManager.s_Dic_Players.TryGetValue(_ID, out PlayerManager _player))
        {
            _player.transform.rotation = _Rotation;
        }
    }

    public static void PlayerDisconnected(Packet iPacket)
    {
        int _ID = iPacket.ReadInt();

        Destroy(GameManager.s_Dic_Players[_ID].gameObject);
        GameManager.s_Dic_Players.Remove(_ID);
    }

    public static void PlayerHealth(Packet iPacket)
    {
        int _ID = iPacket.ReadInt();
        float _Health = iPacket.ReadFloat();

        GameManager.s_Dic_Players[_ID].SetHealth(_Health);
    }

    public static void PlayerRespawned(Packet iPaket)
    {
        int _ID = iPaket.ReadInt();

        GameManager.s_Dic_Players[_ID].Respawn();
    }

    public static void CreateItemSpawner(Packet iPaket)
    {
        int _SpawnerID = iPaket.ReadInt();
        Vector3 _SpawnerPosition = iPaket.ReadVector3();
        bool _IsHasItem = iPaket.ReadBool();

        GameManager.s_instance.CreateItemSpawner(_SpawnerID, _SpawnerPosition, _IsHasItem);
    }

    public static void ItemSpawned(Packet iPaket)
    {
        int _SpawnerID = iPaket.ReadInt();

        GameManager.s_Dic_ItemSpawners[_SpawnerID].ItemSpawned();
    }

    public static void ItemPickedUp(Packet iPaket)
    {
        int _SpawnerID = iPaket.ReadInt();
        int _Player = iPaket.ReadInt();

        GameManager.s_Dic_ItemSpawners[_SpawnerID].ItemPickedUp();
        GameManager.s_Dic_Players[_Player].m_ItemCount++;
    }

    public static void SpawnProjectile(Packet iPaket)
    {
        int _ProjectileID = iPaket.ReadInt();
        Vector3 _Position = iPaket.ReadVector3();
        int _ThrowByPlayer = iPaket.ReadInt();

        GameManager.s_instance.SpawnProjectile(_ProjectileID, _Position);
        GameManager.s_Dic_Players[_ThrowByPlayer].m_ItemCount--;
    }

    public static void ProjectilePosition(Packet iPaket)
    {
        int _ProjectileID = iPaket.ReadInt();
        Vector3 _Position = iPaket.ReadVector3();

        if(GameManager.s_Dic_Projectiles.TryGetValue(_ProjectileID,out ProjectileManager _projectile))
        {
            _projectile.transform.position = _Position;
        }
    }

    public static void ProjectileExploded(Packet iPaket)
    {
        int _ProjectileID = iPaket.ReadInt();
        Vector3 _Position = iPaket.ReadVector3();

        GameManager.s_Dic_Projectiles[_ProjectileID].Explode(_Position);
    }

    public static void SpawnEnemy(Packet iPaket)
    {
        int _enemyID = iPaket.ReadInt();
        Vector3 _Position = iPaket.ReadVector3();

        GameManager.s_instance.SpawnEnemy(_enemyID,_Position);
    }
    public static void EnemyPostiton(Packet iPaket)
    {
        int _enemyID = iPaket.ReadInt();
        Vector3 _Position = iPaket.ReadVector3();

        if(GameManager.s_Dic_Enemys.TryGetValue(_enemyID,out EnemyManager _enemy))
        {
            _enemy.transform.position = _Position;
        }
    }

    public static void EnemyHealth(Packet iPaket)
    {
        int _enemyID = iPaket.ReadInt();
        float _Health = iPaket.ReadFloat();

        GameManager.s_Dic_Enemys[_enemyID].m_Health = _Health;
    }
}
