using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            //creating instance if instance doesn't exist
            if (_instance == null)
            {
                GameObject gm = new GameObject("GameManager");
                gm.AddComponent<GameManager>();
            }

            return _instance;
        }
    }

    public int RocketCollisions { get; set; }
    public int LegCollisions { get; set; }
    public int Collisions { get; private set; } //Sum of RocketCollisons and LegCollisions
    public bool HideDirectionPointer { get; private set; }
    public bool CanRotate { get; private set; } //The players ability to rotate the rocket
    public bool RocketBasedCam { get; set; }

    void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        Collisions = RocketCollisions + LegCollisions;

        if (Collisions > 0)
        {
            HideDirectionPointer = true;
            CanRotate = false;
        }
        else
        {
            HideDirectionPointer = false;
            CanRotate = true;
        }
    }
}
