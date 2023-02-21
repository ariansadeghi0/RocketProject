using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] //Rigidbody required
public class CollisionScript : MonoBehaviour
{
    [Header("Adjustment Values")]
    [SerializeField]private float explosionThreshold;

    [Header("Required Fields")]
    [SerializeField] private ParticleSystem explosion;
    [SerializeField] private Animator explosionLightAnimator;
    
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float impulse = 0f;
        foreach (ContactPoint2D cp in collision.contacts)
        {
            impulse += cp.normalImpulse;
        }
        if (impulse >= explosionThreshold)
        {
            rb.velocity = new Vector2(0, 0);//Stopping rocket

            explosion.Play();//EXPLODE
            explosionLightAnimator.SetTrigger("Explode");
        }

        GameManager.Instance.RocketCollisions += 1;
    }

    private void OnCollisionExit2D()
    {
        GameManager.Instance.RocketCollisions -= 1;
    }
}
