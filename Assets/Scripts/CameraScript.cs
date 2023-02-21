using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public bool rocketBased = true;
    public GameObject rocket;
    [SerializeField] private bool planetBasedCam;

    [Header("Background Values")]
    [SerializeField] private bool backgroundEnabled;
    [SerializeField] private Transform background;

    private float angle;
    private Vector2 dir = new Vector2(0, 1);
    private bool planetFocused;

    private Camera camComponent;
    [SerializeField] float cameraDistanceMax = 20f;
    [SerializeField] float cameraDistanceMin = 5f;
    [SerializeField] float scrollSpeed = 0.5f;
    public bool smoothFollow;
    [SerializeField] private float camFollowSmoothTime = 0.05F;

    private Vector3 initialCamPosition;
    private float initialCamSize;
    private Vector3 initialBackgroundScale;

    private void Awake()
    {
        camComponent = GetComponent<Camera>();

        initialCamPosition = transform.position;
        initialCamSize = camComponent.orthographicSize;
        initialBackgroundScale = background.localScale;
    }

    private void Start()
    {
        if (!planetBasedCam)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            planetFocused = true;
        }

        if (backgroundEnabled)
        {
            float camSize = GetComponent<Camera>().orthographicSize;

            background.localScale = new Vector3(camSize / 7, camSize / 7);
        }
    }

    private void Update()
    {
        if (rocketBased && !smoothFollow)
        {
            transform.position = new Vector3(rocket.transform.position.x, rocket.transform.position.y, transform.position.z);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!rocketBased)
            {
                rocketBased = true;
                GameManager.Instance.RocketBasedCam = true;
            }
            else if (rocketBased)
            {
                rocketBased = false;
                GameManager.Instance.RocketBasedCam = false;
                transform.position = initialCamPosition;
                camComponent.orthographicSize = initialCamSize;
                background.localScale = initialBackgroundScale;
            }
        }
    }

    void FixedUpdate()
    {
        if (planetBasedCam && Input.GetKeyDown(KeyCode.RightControl))
        {
            planetFocused = !planetFocused; //Switching camera mode if
            if (!planetFocused)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        if (backgroundEnabled)
        {
            //Setting background position
            background.position = new Vector3(transform.position.x, transform.position.y);
        }

        CheckSizeChangeInput();

        if (rocketBased)
        {
            if (smoothFollow)
            {
                Vector3 velocity = Vector3.zero;
                // Smoothly moving the camera towards rocket
                transform.position = Vector3.SmoothDamp(transform.position, new Vector3(rocket.transform.position.x, rocket.transform.position.y, transform.position.z), ref velocity, camFollowSmoothTime);
            }

            if (planetFocused)  //Setting camera rotation
            {
                angle = Vector2.SignedAngle(rocket.transform.position, dir);
                transform.rotation = Quaternion.Euler(0, 0, -angle);
            }
        }
    }

    void CheckSizeChangeInput()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
        {
            //Do nothing
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            if (camComponent.orthographicSize + scrollSpeed * Time.deltaTime < cameraDistanceMax)
            {
                camComponent.orthographicSize += scrollSpeed * Time.deltaTime;
                if (backgroundEnabled)
                {
                    float camSize = GetComponent<Camera>().orthographicSize;

                    background.localScale = new Vector3(camSize / 7, camSize / 7);
                }
            }
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            if (camComponent.orthographicSize - scrollSpeed * Time.deltaTime > cameraDistanceMin)
            {
                camComponent.orthographicSize -= scrollSpeed * Time.deltaTime;
                if (backgroundEnabled)
                {
                    float camSize = GetComponent<Camera>().orthographicSize;

                    background.localScale = new Vector3(camSize / 7, camSize / 7);
                }
            }
        }
    }
}
