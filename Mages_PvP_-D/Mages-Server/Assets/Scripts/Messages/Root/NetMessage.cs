using Unity.Networking.Transport;
using UnityEngine;
using System;
using MagesServer.DataLayer;
using MagesServer.Enums;

namespace MagesServer.DataLayer
{
    public class NetMessage
    {
        private sbyte Code;
        private object[] objArray;
        protected GameState gameState = GameState.All;

        public NetMessage(OpCode code, object[] objarray)
        {
            Code = (sbyte)code;
            objArray = objarray;
        }

        public NetMessage()
        {
            gameState = GameState.Ended;
        }

        public NetMessage(ref DataStreamReader reader)
        {
            if(!Deserialise(ref reader))
            {
                Debug.LogError($"SERVER:: Unexpected message!");
            }
        }

        public void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)Code);
            for (int i = 0; i < objArray.Length; i++)
            {
                if(objArray[i] != null)
                {
                    switch (Type.GetTypeCode(objArray[i].GetType()))
                    {
                        case TypeCode.Int32: writer.WriteInt((int)objArray[i]); break;
                        case TypeCode.Double: writer.WriteFloat((float)(double)objArray[i]); break;
                        case TypeCode.Decimal: writer.WriteFloat((float)(decimal)objArray[i]); break;
                        case TypeCode.Boolean: writer.WriteByte((byte)objArray[i]); break;
                        case TypeCode.Char: writer.WriteByte((byte)(char)objArray[i]); break;
                        case TypeCode.String: writer.WriteFixedString64((string)objArray[i]); break;
                        case TypeCode.Byte: writer.WriteByte((byte)objArray[i]); break;
                        case TypeCode.UInt16: writer.WriteUShort((ushort)objArray[i]); break;
                        default:
                            Debug.LogError("Error type matching. Unknown type");
                            break;
                    }
                }
                else
                {
                    Debug.LogError("object was null");
                }
            }
            
        }

        public virtual bool Deserialise(ref DataStreamReader reader)
        {
            if (gameState == GameState.All || GameServer.GameState == gameState)
                return true;
            return false;
        }

        public virtual void Received(NetworkConnection c)
        {
            Debug.LogError("No implentation for received or unexpected message");
        }

        public void ReverseCode()
        {
            Code = (sbyte)-Code;
        }
    }
}
