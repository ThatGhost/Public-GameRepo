using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using MagesServer.DataLayer;

namespace MagesServer.CallBackLayer
{
    public class ServerInterface : MonoBehaviour
    {
        private GameServer _gameServer;
        private ConfirmationHandler _confirmationHandler;
        private PlayerManager _playerManager;

        void Awake()
        {
            _gameServer = GetComponent<GameServer>();
            _confirmationHandler = GetComponent<ConfirmationHandler>();
            _playerManager = GetComponent<PlayerManager>();

            NET_Confirmation.onReceived += OnCofirmation;
            NET_Time.onReceived += OnTime;
        }

        private void OnCofirmation(NetMessage message, NetworkConnection conn)
        {
            NET_Confirmation msg = (NET_Confirmation)message;
            _confirmationHandler.HandleConfirmation(msg.Type, conn);
        }

        private void OnTime(NetMessage message, NetworkConnection conn)
        {
            NET_Time msg = (NET_Time)message;
            msg = new NET_Time(new object[] { (double)msg.clientTime, (double)_gameServer.GetTime() });
            _gameServer.SendToClient(msg, conn);
        }
    }
}