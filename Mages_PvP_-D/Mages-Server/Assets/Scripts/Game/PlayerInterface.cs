using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagesServer.GameLayer.Magic;

namespace MagesServer.GameLayer
{
    public class PlayerInterface : MonoBehaviour
    {
        private PlayerController _playerController;
        private PlayerSpellInterface _playerSpellInterface;
        private PlayerData _playerData;

        void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _playerSpellInterface = GetComponent<PlayerSpellInterface>();
            _playerData = GetComponent<PlayerData>();
        }

        public void PlayerInput(List<InputBits> inputs)
        {
            _playerController.AddInputs(inputs);
            _playerSpellInterface.AddInputs(inputs);
        }

        public void AddHP(int damage) => _playerData.AddHp(damage);
        public void AddMana(int mana) => _playerData.AddMana(mana);
        public void SetPos(Vector3 pos) => _playerController.SetPosition(IntVector3D.Convert(pos, 1000));
        public void SetTeam(char team) => _playerData.SetTeam(team);

        public float GetFaceAngle()
        {
            return _playerController._RotationX;
        }
        public IntVector3D GetPosition() => _playerController._position;
        public IntVector3D GetVelocity() => _playerController._velocity;
        public byte GetStates()
        {
            byte states = 0;

            if (_playerController._Running)
                states |= 1;
            if (_playerController._Crouching)
                states |= 2;
            if (!_playerController._Grounded)
                states |= 4;
            return states;
        }
        public byte GetTriggeredAbilities() => _playerSpellInterface.GetTriggeredAbilities();
        public bool HasEnoughMana(int mana) => _playerData.HasEnoughMana(mana);
        public int GetHP() => _playerData.GetHP();
        public int GetMana() => _playerData.GetMana();
        public char GetTeam() => _playerData.GetTeam();
    }
}
