using MagesClient.DataLayer;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MagesClient.CallBackLayer
{
    public class ClientInterface : MonoBehaviour
    {
        private GameClient _gameClient;
        private ConfirmationHandler _confirmationHandler;
        private PlayerManager _playerManager;
        private int _timingIntervalRate = 5; //in seconds

        void Start()
        {
            _gameClient = GetComponent<GameClient>();
            _confirmationHandler = GetComponent<ConfirmationHandler>();
            _playerManager = GetComponent<PlayerManager>();

            _timingIntervalRate = _gameClient.GetTickRate() * _timingIntervalRate;

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            _gameClient.onTick += OnTick;
            NET_Confirmation.onReceived += OnCofirmation;
            NET_Time.onReceived += OnTime;
            NET_PFHandshake.onReceived += OnPFHandshake;
            NET_ServerDetails.onReceived += OnServerDetails;
        }

        private void OnTick(float time, int tick)
        {
            if (tick % _timingIntervalRate == 0)
            {
                _gameClient.SendToServer(new NET_Time(new object[] { (double)_gameClient.GetTime() }));
            }
        }

        private void OnCofirmation(NetMessage message)
        {
            NET_Confirmation msg = (NET_Confirmation)message;
            _confirmationHandler.HandleConfirmation(msg.Type);
        }

        private void OnTime(NetMessage message)
        {
            NET_Time msg = (NET_Time)message;
            float ping = msg.clientTime - msg.serverTime + (msg.serverTime - _gameClient.GetTime());
            ping /= 2f;
            _gameClient.SetTime(msg.serverTime + ping);
        }

        private void OnPFHandshake(NetMessage message)
        {
            _gameClient.SendToServer(new NET_PFHandshake(new object[] { "A3B24O95C2JH" }));
        }

        private void OnServerDetails(NetMessage message)
        {
            NET_ServerDetails msg = (NET_ServerDetails)message;
            //_gameClient.SetTickRate(msg.ServerTickRate);
            //_gameClient.SetTickRate(msg.ServerTickRate);
        }
    }
}