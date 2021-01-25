using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace GameServer
{
    class Player
    {
        public int m_ID;
        public string m_UserName;

        public Vector3 m_Postiotn;
        public Quaternion m_Rotation;

        private float m_MoveSpeed = 5f / Constants.TICKS_PER_SEC;
        private bool[] m_Ay_Inputs;

        public Player(int iID,string iUserName,Vector3 iPosition)
        {
            m_ID = iID;
            m_UserName = iUserName;
            m_Postiotn = iPosition;
            m_Rotation = Quaternion.Identity;

            m_Ay_Inputs = new bool[4];
        }

        public void Update()
        {
            Vector2 _InputDirection = Vector2.Zero;
            if (m_Ay_Inputs[0])
            {
                _InputDirection.Y += 1;
            }
            if (m_Ay_Inputs[1])
            {
                _InputDirection.Y -= 1;
            }
            if (m_Ay_Inputs[2])
            {
                _InputDirection.X += 1;
            }
            if (m_Ay_Inputs[3])
            {
                _InputDirection.X -= 1;
            }

            Move(_InputDirection);
        }

        private void Move(Vector2 iInputDirection)
        {
            Vector3 _Forward = Vector3.Transform(new Vector3(0, 0, 1), m_Rotation);
            Vector3 _Right = Vector3.Normalize(Vector3.Cross(_Forward, new Vector3(0, 1, 0)));

            Vector3 _MoveDirection = _Right * iInputDirection.X + _Forward * iInputDirection.Y;

            m_Postiotn += _MoveDirection * m_MoveSpeed;

            ServerSend.PlayerPosition(this);
            ServerSend.PlayerRotation(this);
        }

        public void SetInput(bool[] iInputs,Quaternion iRotation)
        {
            m_Ay_Inputs = iInputs;
            m_Rotation = iRotation;
        }
    }
}
