using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] //Rigidbody required
public class RotationScript : MonoBehaviour
{
    [Header("Adjustment Values")]
    [SerializeField] private float rotationSpeedMultiplier;  //Used for mouse control
    [SerializeField] [Tooltip("Angular drag applied when the player is rotating the rocket")]
    private float rotatingAngularDrag;

    [Header("Required Fields")]
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform bottomPoint;

    private Vector2 mouseWorldPosition;
    private Vector2 rocketPosition;
    private Vector2 rocketOrientationDir;
    private Vector2 mouseToRocketDir;
    private Rigidbody2D rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.centerOfMass = new Vector2(0, 0);
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.CanRotate)
        {
            mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rocketPosition = transform.position;

            rocketOrientationDir = (new Vector2(topPoint.position.x, topPoint.position.y) - new Vector2(bottomPoint.position.x, bottomPoint.position.y)).normalized;
            mouseToRocketDir = (mouseWorldPosition - rocketPosition).normalized;

            float mouseToRocketAngle = Vector2.SignedAngle(mouseToRocketDir, rocketOrientationDir);
            if (mouseToRocketAngle < 0)
            {
                mouseToRocketAngle += 360;
            }

            float rocketRotation = transform.rotation.eulerAngles.z;
            float targetRocketAngle = rocketRotation - mouseToRocketAngle;
            float diff = targetRocketAngle - rocketRotation;

            if (diff < -180)
            {
                diff += 360;
            }

            rb.AddTorque(diff * rotationSpeedMultiplier);
            rb.angularDrag = rotatingAngularDrag;
        }
    }
}
