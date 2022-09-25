using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    [SerializeField] IntVector3D direction;
    List<PlayerController> playerCollisions = new List<PlayerController>();

    private void FixedUpdate()
    {
        foreach (var item in playerCollisions)
        {
            if (item != null)
            {
                item.AddForce(direction, ForceMode.Force);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            playerCollisions.Add(other.gameObject.GetComponent<PlayerController>());
            player.SetJumping(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
            playerCollisions.Remove(other.gameObject.GetComponent<PlayerController>());
    }
}
