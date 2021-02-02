using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{
    private static void SendTCPData(int iClient, Packet iPacket)
    {
        iPacket.WriteLength();
        Server.s_Dic_Clients[iClient].m_TCP.SendData(iPacket);
    }

    private static void SendUDPData(int iClient, Packet iPacket)
    {
        iPacket.WriteLength();
        Server.s_Dic_Clients[iClient].m_UDP.SendData(iPacket);
    }

    private static void SendTCPDataToAll(Packet iPacket)
    {
        iPacket.WriteLength();
        for (int i = 1; i <= Server.s_MaxPlayers; i++)
        {
            Server.s_Dic_Clients[i].m_TCP.SendData(iPacket);
        }
    }
    private static void SendTCPDataToAll(int iExceptClient, Packet iPacket)
    {
        iPacket.WriteLength();
        for (int i = 1; i <= Server.s_MaxPlayers; i++)
        {
            if (i != iExceptClient)
            {
                Server.s_Dic_Clients[i].m_TCP.SendData(iPacket);
            }
        }
    }

    private static void SendUDPDataToAll(Packet iPacket)
    {
        iPacket.WriteLength();
        for (int i = 1; i <= Server.s_MaxPlayers; i++)
        {
            Server.s_Dic_Clients[i].m_UDP.SendData(iPacket);
        }
    }
    private static void SendUDPDataToAll(int iExceptClient, Packet iPacket)
    {
        iPacket.WriteLength();
        for (int i = 1; i <= Server.s_MaxPlayers; i++)
        {
            if (i != iExceptClient)
            {
                Server.s_Dic_Clients[i].m_UDP.SendData(iPacket);
            }
        }
    }


    #region Packet
    public static void Welcome(int iClient, string iMessage)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(iMessage);
            _packet.Write(iClient);

            SendTCPData(iClient, _packet);
        }
    }

    public static void SpawnPlayer(int iClient, Player iPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(iPlayer.m_ID);
            _packet.Write(iPlayer.m_UserName);
            _packet.Write(iPlayer.transform.position);
            _packet.Write(iPlayer.transform.rotation);

            SendTCPData(iClient, _packet);
        }
    }

    public static void PlayerPosition(Player iPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(iPlayer.m_ID);
            _packet.Write(iPlayer.transform.position);

            SendUDPDataToAll(_packet);
        }
    }

    public static void PlayerRotation(Player iPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(iPlayer.m_ID);
            _packet.Write(iPlayer.transform.rotation);

            SendUDPDataToAll(iPlayer.m_ID, _packet);
        }
    }

    public static void PlayerDisconnected(int iPlayerID)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(iPlayerID);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerHealth(Player iPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerHealth))
        {
            _packet.Write(iPlayer.m_ID);

            SendTCPDataToAll(_packet);
        }
    }

    public static void PlayerRespawned(Player iPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRespawned))
        {
            _packet.Write(iPlayer.m_ID);
            _packet.Write(iPlayer.m_Health);

            SendTCPDataToAll(_packet);
        }
    }

    public static void CreateItemSpawner(int iClient, int iSpawnerID,Vector3 iSpawnerPosition, bool iIsHasItem)
    {
        using (Packet _packet = new Packet((int)ServerPackets.createItemSpawner))
        {
            _packet.Write(iSpawnerID);
            _packet.Write(iSpawnerPosition);
            _packet.Write(iIsHasItem);

            SendTCPData(iClient,_packet);
        }
    }

    public static void ItemSpawned(int iSpawnerID)
    {
        using (Packet _packet = new Packet((int)ServerPackets.itemSpawned))
        {
            _packet.Write(iSpawnerID);

            SendTCPDataToAll(_packet);
        }
    }

    public static void ItemPickedUp(int iSpawnerID,int iPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.itemPickedUp))
        {
            _packet.Write(iSpawnerID);
            _packet.Write(iPlayer);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SpawnProjectile(Projectile iProjectile, int iThrowByPlayer)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(iProjectile.m_ID);
            _packet.Write(iThrowByPlayer);
            _packet.Write(iThrowByPlayer);

            SendTCPDataToAll(_packet);
        }
    }

    public static void ProjectilePosition(Projectile iProjectile)
    {
        using (Packet _packet = new Packet((int)ServerPackets.projectilePosition))
        {
            _packet.Write(iProjectile.m_ID);
            _packet.Write(iProjectile.transform.position);

            SendUDPDataToAll(_packet);
        }
    }

    public static void ProjectileExploded(Projectile iProjectile)
    {
        using (Packet _packet = new Packet((int)ServerPackets.projectileExploded))
        {
            _packet.Write(iProjectile.m_ID);
            _packet.Write(iProjectile.transform.position);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SpaenEnemy(Enemy iEnemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnEnemy))
        {
            SendTCPDataToAll(SpawnEnemy_Data(iEnemy,_packet));
        }
    }

    public static void SpaenEnemy(int iClient, Enemy iEnemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnEnemy))
        {
            SendTCPDataToAll(iClient, SpawnEnemy_Data(iEnemy, _packet));
        }
    }

    private static Packet SpawnEnemy_Data(Enemy iEnemy,Packet iPacket)
    {
        iPacket.Write(iEnemy.m_ID);
        iPacket.Write(iEnemy.transform.position);
        return iPacket;
    }

    public static void EnemyPsoition(Enemy iEnemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemyPositon))
        {
            _packet.Write(iEnemy.m_ID);
            _packet.Write(iEnemy.transform.position);

            SendUDPDataToAll(_packet);
        }
    }

    public static void EnemyHealth(Enemy iEnemy)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemyHealth))
        {
            _packet.Write(iEnemy.m_ID);
            _packet.Write(iEnemy.m_Health);

            SendTCPDataToAll(_packet);
        }
    }
    #endregion
}
