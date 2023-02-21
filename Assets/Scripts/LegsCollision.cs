using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegsCollision : MonoBehaviour
{
    private static int collisions = 0;

    private void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.transform.CompareTag("LandingLeg") == false)
        {
            collisions += 1;
            GameManager.Instance.LegCollisions = collisions;
        }
    }
    private void OnCollisionExit2D(Collision2D collider)
    {
        if (collider.transform.CompareTag("LandingLeg") == false)
        {
            collisions -= 1;
            GameManager.Instance.LegCollisions = collisions;
        }
    }
}
