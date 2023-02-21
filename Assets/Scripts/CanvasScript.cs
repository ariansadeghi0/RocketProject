using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasScript : MonoBehaviour
{
    [Header("Required Fields")]
    [SerializeField] private GameObject rocket;
    [SerializeField] private bool isAgent;
    [SerializeField] private FinalRocketAgent agentScript;
    [SerializeField] private Transform dashedCircle;
    [SerializeField] private Image directionPointer;
    [SerializeField] private Transform orientationPointer;
    [SerializeField] private TextMeshProUGUI speedText;

    private Rigidbody2D rocketRb;

    private void Awake()
    {
        rocketRb = rocket.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        UpdateDirectionPointer();
        UpdateOrientationPointer();
        UpdateSpeedText();

        if (GameManager.Instance.RocketBasedCam)
        {
            dashedCircle.gameObject.SetActive(true);
            directionPointer.gameObject.SetActive(true);
            orientationPointer.gameObject.SetActive(true);
            speedText.gameObject.SetActive(true);
        }
        else
        {
            dashedCircle.gameObject.SetActive(false);
            directionPointer.gameObject.SetActive(false);
            orientationPointer.gameObject.SetActive(false);
            speedText.gameObject.SetActive(false);
        }
    }

    void UpdateDirectionPointer()
    {
        if (GameManager.Instance.HideDirectionPointer)
        {
            directionPointer.enabled = false;
        }
        else if (isAgent && agentScript.legsInCollisionCount > 0)
        {
            directionPointer.enabled = false;
        }
        else
        {
            directionPointer.enabled = true;

            Vector2 dir = rocketRb.velocity;
            float angle = Vector2.SignedAngle(new Vector3(1, 0), dir);
            directionPointer.transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }

    void UpdateOrientationPointer()
    {
        orientationPointer.eulerAngles = new Vector3(0, 0, rocketRb.transform.eulerAngles.z);
    }

    void UpdateSpeedText()
    {
        speedText.text = rocketRb.velocity.magnitude.ToString("F2") + "m/s";
    }
}
