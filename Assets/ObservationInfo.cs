using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ObservationInfo : MonoBehaviour
{
    [Header("Required Gameobjects")]
    [SerializeField] private GameObject rocket;
    [SerializeField] private GameObject landingPad;

    [Header("Text Fields")]
    [SerializeField] private TextMeshProUGUI hVelocityText;
    [SerializeField] private TextMeshProUGUI vVelocityText;
    [SerializeField] private TextMeshProUGUI aVelocityText;
    [SerializeField] private TextMeshProUGUI rocketRotText;
    [SerializeField] private TextMeshProUGUI rocketPosText;
    [SerializeField] private TextMeshProUGUI landingPadPosText;
    [SerializeField] private TextMeshProUGUI padToRocketDistText;

    private Rigidbody2D rocketRb;
    private float rocketRot;

    private void Start()
    {
        rocketRb = rocket.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rocketRot = rocket.transform.rotation.eulerAngles.z;
        if (rocketRot > 180)
        {
            rocketRot -= 360;
        }

        hVelocityText.text = rocketRb.velocity.x.ToString("F2") + " units/s";
        vVelocityText.text = rocketRb.velocity.y.ToString("F2") + " units/s";
        aVelocityText.text = rocketRb.angularVelocity.ToString("F2") + " degrees/s";
        rocketRotText.text = rocketRot.ToString("F2") + " degrees";
        rocketPosText.text = "( " + rocket.transform.position.x.ToString("F2") + " , " + rocket.transform.position.y.ToString("F2") + " )";
        landingPadPosText.text = "( " + landingPad.transform.position.x.ToString("F2") + " , " + landingPad.transform.position.y.ToString("F2") + " )";
        padToRocketDistText.text = Vector2.Distance(rocket.transform.position, landingPad.transform.position).ToString("F2") + " units";
    }
}
