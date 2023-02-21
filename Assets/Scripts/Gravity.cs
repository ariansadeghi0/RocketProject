using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    [SerializeField] private float intensity;

    private float planetMass;

    private float dist;
    private Vector2 pullForce;

    private void Start()
    {
        planetMass = GetComponent<Rigidbody2D>().mass;
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();

            dist = Vector2.Distance(transform.position, collider.transform.position);
            pullForce = (transform.position - collider.transform.position).normalized * (intensity * ((planetMass * rb.mass) / (dist * dist)));
            rb.AddForce(pullForce, ForceMode2D.Force);

            /*dist = Vector2.Distance(transform.position, collider.transform.position);
            pullForce = (transform.position - collider.transform.position).normalized / dist * intensity;
            collider.GetComponent<Rigidbody2D>().AddForce(pullForce, ForceMode2D.Force);
            */
        }
    }
}
