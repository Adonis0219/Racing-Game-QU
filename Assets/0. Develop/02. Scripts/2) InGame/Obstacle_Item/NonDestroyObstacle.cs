using System;
using Unity.VisualScripting;
using UnityEngine;

public class NonDestroyObstacle : IObstacle
{
    private void OnTriggerExit(Collider other)
    {
        if (player.isChargeMoving) player.isChargeMoving = false;
    }
}
