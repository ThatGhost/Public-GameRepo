using Unity.Networking.Transport;
using UnityEngine;
using System;
using MagesClient.Enums;

namespace MagesClient.DataLayer
{
    public class NetMessage
    {
        private sbyte _code; //128 types of diffrent messages possible
        private object[] _objArray;
        protected GameState _GameState = GameState.All;

        public NetMessage(OpCode code, object[] objarray)
        {
            _code = (sbyte)code;
            _objArray = objarray;
        }

        public NetMessage()
        {
            _GameState = GameState.Ended;
        }

        public NetMessage(ref DataStreamReader reader)
        {
            if (!Deserialise(ref reader))
            {
                Debug.LogError($"SERVER:: Unexpected message!");
            }
        }

        public void Serialize(ref DataStreamWriter writer)
        {
            writer.WriteByte((byte)_code);
            for (int i = 0; i < _objArray.Length; i++)
            {
                switch (Type.GetTypeCode(_objArray[i].GetType()))
                {
                    case TypeCode.Int32: writer.WriteInt((int)_objArray[i]); break;
                    case TypeCode.Double: writer.WriteFloat((float)(double)_objArray[i]); break;
                    case TypeCode.Decimal: writer.WriteFloat((float)(decimal)_objArray[i]); break;
                    case TypeCode.Boolean: writer.WriteByte((byte)_objArray[i]); break;
                    case TypeCode.Char: writer.WriteByte((byte)(char)_objArray[i]); break;
                    case TypeCode.String: writer.WriteFixedString128((string)_objArray[i]); break;
                    case TypeCode.Byte: writer.WriteByte((byte)_objArray[i]); break;
                    case TypeCode.UInt16: writer.WriteUShort((ushort)_objArray[i]); break;
                    default:
                        Debug.LogError("Error type matching. Unknown type");
                        break;
                }
            }
        }

        public virtual bool Deserialise(ref DataStreamReader reader)
        {
            if (_GameState == GameState.All || GameClient.GameState == _GameState)
                return true;
            return false;
        }

        public virtual void Received()
        {
            Debug.LogError("No implentation for received or unexpected message");
        }

        public void ReverseCode()
        {
            _code = (sbyte)-_code;
        }
    }
}
