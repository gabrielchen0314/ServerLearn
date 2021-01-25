using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer
{
    class GameLogic
    {
        public static void Update()
        {
            foreach (Client _Client in Server.s_Dic_Clients.Values)
            {
                if(_Client.m_Player != null)
                {
                    _Client.m_Player.Update();
                }
            }

            ThreadManager.UpdateMain();
        }
    }
}
