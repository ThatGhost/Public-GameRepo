using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    [SerializeField] IntVector3D direction;
    OwnerController ownerController;

    private void FixedUpdate()
    {
        if(ownerController != null)
        {
            ownerController.AddForce(direction,ForceMode.Force);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ownerController = other.gameObject.GetComponent<OwnerController>();
        if (ownerController != null)
            ownerController.SetJumping(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.GetComponent<OwnerController>())
            ownerController = null;
    }
}
