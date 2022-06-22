
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace MagesClient.GameLayer
{
    public class OwnerInterface : MonoBehaviour
    {
        private OwnerController _ownerController;
        private const int _maxDistanceBeforePopping = 5;

        void Start()
        {
            _ownerController = GetComponent<OwnerController>();
        }

        public void ReceveidPoistion(Vector3 position)
        {
            _ownerController.SetPrevPosition(position);
            if(Vector3.Distance(_ownerController.GetPrevPosition(), transform.position) > _maxDistanceBeforePopping)
            {
                transform.position = _ownerController.GetPrevPosition();
            }
            Debug.Log($"diff in server vs client {Vector3.Distance(_ownerController.GetPrevPosition(), transform.position)}, {position}");
        }

        public void ReceivedState(byte b)
        {
            //_ownerController.OnStateChange(b);
        }
    }
}