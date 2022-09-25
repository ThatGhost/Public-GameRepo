
using MagesClient.GameLayer.Magic;
using UnityEngine;

namespace MagesClient.GameLayer
{
    public class PlayerInterface : MonoBehaviour
    {
        private PlayerController _playerController;
        private PlayerSpellController _playerSpellController;

        private char _team;

        public char Team
        {
            get { return _team; }
            set { _team = value; GetComponent<PlayerSpellController>().SetAllTeams(_team); }
        }

        void Start()
        {
            _playerSpellController = GetComponent<PlayerSpellController>();
            _playerController = GetComponent<PlayerController>();
        }

        public void ReceivedPosition(Vector3 position, Vector2 mouse, byte state)
        {
            _playerController.OnSnapshotPosition(position, mouse, state);
        }

        public void ReceivedAbilities(byte abilities)
        {
            _playerSpellController.TriggerAbilities(abilities);
        }
    }
}