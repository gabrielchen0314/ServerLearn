using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.s_Dic_Clients[_fromClient].m_TCP.m_Socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }
        Server.s_Dic_Clients[_fromClient].SendIntoGame(_username);
    }

    public static void PlayerMovent(int _fromClient, Packet _packet)
    {
        bool[] _Inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _Inputs.Length; i++)
        {
            _Inputs[i] = _packet.ReadBool();
        }

        Quaternion _Rotation = _packet.ReadQuaternion();

        Server.s_Dic_Clients[_fromClient].m_Player.SetInput(_Inputs, _Rotation);
    }

    public static void PlayerShoot(int _fromClient,Packet iPacked)
    {
        Vector3 _ShootDirection = iPacked.ReadVector3();

        Server.s_Dic_Clients[_fromClient].m_Player.Shoot(_ShootDirection);
    }

    public static void PlayerThorwItem(int _fromClient, Packet iPacked)
    {
        Vector3 _ShootDirection = iPacked.ReadVector3();

        Server.s_Dic_Clients[_fromClient].m_Player.ThrowItem(_ShootDirection);
    }
}

