using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractButton : BaseInteractionScr
{
    private Vector3 _startPos;
    private bool _toggle = false;

    private void Start()
    {
        _startPos = transform.position;
    }

    public override void OnInteraction(GameObject Interacter)
    {
        print("Interacted with button");
        if(_toggle)
        {
            _toggle = false;
            transform.position = _startPos - new Vector3(-0.2f,0,0);
        }
        else
        {
            _toggle=true;
            transform.position = _startPos;
        }
    }
}
