
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagesServer.GameLayer
{
    public abstract class BaseInteractionScr : MonoBehaviour
    {
        public abstract void OnInteraction(GameObject Interacter);
    }
}