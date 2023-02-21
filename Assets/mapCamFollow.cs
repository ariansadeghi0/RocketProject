using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapCamFollow : MonoBehaviour
{
    [SerializeField] private Transform rocket;

    void Update()
    {
        transform.position = new Vector3(rocket.position.x, transform.position.y, transform.position.z);
    }
}
