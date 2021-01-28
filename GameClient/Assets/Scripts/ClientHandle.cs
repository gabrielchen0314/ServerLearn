using System.Collections;
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

        GameManager.s_Dic_Players[_ID].transform.position = _Position;
    }
    public static void PlayerRotation(Packet iPacket)
    {
        int _ID = iPacket.ReadInt();
        Quaternion _Rotation = iPacket.ReadQuaternion();

        GameManager.s_Dic_Players[_ID].transform.rotation = _Rotation;
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
}
