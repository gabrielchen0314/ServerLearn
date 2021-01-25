using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Console.WriteLine($"{Server.s_Dic_Clients[_fromClient].m_TCP.m_Socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }
            Server.s_Dic_Clients[_fromClient].SendIntoGame(_username);
        }

        public static void PlayerMovent(int _fromClient, Packet _packet)
        {
            bool[] _Inputs = new bool[_packet.ReadInt()];
            for(int i = 0; i < _Inputs.Length; i++)
            {
                _Inputs[i] = _packet.ReadBool();
            }

            Quaternion _Rotation = _packet.ReadQuaternion();

            Server.s_Dic_Clients[_fromClient].m_Player.SetInput(_Inputs,_Rotation);
        }
    }
}
