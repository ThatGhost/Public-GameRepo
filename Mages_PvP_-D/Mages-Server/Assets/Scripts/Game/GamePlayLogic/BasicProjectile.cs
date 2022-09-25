using System;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    [NonSerialized] public int Attack;
    [NonSerialized] public char Team;
    [NonSerialized] public GameObject Owner;
    protected MagesServer.CallBackLayer.PlayerManager _playerManager;

    public void SetPlayerManager(MagesServer.CallBackLayer.PlayerManager manager)
    {
        _playerManager = manager;
    }

    protected virtual void OnCollision(Collision coll)
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject != Owner)
            OnCollision(collision);
    }
}
