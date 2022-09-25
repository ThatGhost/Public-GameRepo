using MagesClient.DataLayer;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace MagesClient.CallBackLayer
{
    public class ClientInterface : MonoBehaviour
    {
        private GameClient _gameClient;
        private ConfirmationHandler _confirmationHandler;
        [SerializeField] private int _timingIntervalRate; //in ticks

        void Start()
        {
            _gameClient = GetComponent<GameClient>();
            _confirmationHandler = GetComponent<ConfirmationHandler>();

            QualitySettings.vSyncCount = 0;
            //Application.targetFrameRate = 60;

            GameClient.onTick += OnTick;
            NET_Confirmation.onReceived += OnCofirmation;
            NET_Time.onReceived += OnTime;
            NET_PFHandshake.onReceived += OnPFHandshake;
            NET_ServerDetails.onReceived += OnServerDetails;
        }

        private void OnTick(uint tick)
        {
            if (tick % _timingIntervalRate == 0)
            {
                _gameClient.SendToServer(new NET_Time(new object[] { (int)GameClient.Tick }));
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

            if(msg.serverTick != GameClient.Tick)
            {
                uint CorrectTick = msg.serverTick;

                if (msg.clientTick != 0)
                {
                    CorrectTick += (msg.serverTick - msg.clientTick);
                }
                _gameClient.SetTick(CorrectTick);
            }
            _gameClient.SetPing((msg.serverTick - msg.clientTick) * Time.fixedDeltaTime * 1000);
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