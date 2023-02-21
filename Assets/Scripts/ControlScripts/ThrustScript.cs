using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] //Rigidbody required
public class ThrustScript : MonoBehaviour
{
    [Header("Adjustment Values")]
    [SerializeField] private float thrustMultiplier;
    [SerializeField] private float boostThrustMultiplier;
    [SerializeField] private float exhaustMaxDistance;
    [SerializeField] private float exhaustMaxLifetime;

    [Header("Required Fields")]
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform bottomPoint;
    [SerializeField] private ParticleSystem exhaust;
    [SerializeField] private Animator exhaustLightAnimator;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
            if (Input.GetMouseButton(0))
            {
                HandleExhaustOn();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                HandleExhaustOff();
            }
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0))    //Normal thrust
        {
            Vector3 thrust = (topPoint.position - bottomPoint.position) * thrustMultiplier;
            rb.AddForce(thrust);
        }
    }

    void HandleExhaustOn()
    {
        var main = exhaust.main;
        //Calculating particle lifetime to match max exhaust distance
        float targetLifetime = exhaustMaxDistance / rb.velocity.magnitude;

        if (targetLifetime > exhaustMaxLifetime)
        {
            main.startLifetime = exhaustMaxLifetime;
        }
        else
        {
            main.startLifetime = targetLifetime;
        }

        if (!exhaust.main.loop)
        {
            main.loop = true;
            exhaust.Play();

            exhaustLightAnimator.SetBool("EngineOn", true);
        }
    }
    void HandleExhaustOff()
    {
        if (exhaust.main.loop)
        {
            var main = exhaust.main;
            main.loop = false;

            exhaustLightAnimator.SetBool("EngineOn", false);
        }
    }
}
