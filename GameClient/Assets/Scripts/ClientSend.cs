using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet iPacket)
    {
        iPacket.WriteLength();
        Client.s_instance.m_TCP.SendData(iPacket);
    }

    private static void SendUDPData(Packet iPacket)
    {
        iPacket.WriteLength();
        Client.s_instance.m_UDP.SendData(iPacket);
    }

    #region Packets
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.s_instance.m_MyID);
            _packet.Write(UIManager.instance.usernameField.text);

            SendTCPData(_packet);
        }
    }

    public static void PlayerMovement(bool[] iInputs)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(iInputs.Length);
            foreach(bool _Input in iInputs)
            {
                _packet.Write(_Input);
            }
            _packet.Write(GameManager.s_Dic_Players[Client.s_instance.m_MyID].transform.rotation);

            SendUDPData(_packet);
        }
    }

    public static void PlayerShoot( Vector3 iFacing)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerShoot))
        {
            _packet.Write(iFacing);

            SendTCPData(_packet);
        }
    }

    public static void PlayerThrowItem(Vector3 iFacing)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerThrowItem))
        {
            _packet.Write(iFacing);

            SendTCPData(_packet);
        }
    }
    #endregion
}
