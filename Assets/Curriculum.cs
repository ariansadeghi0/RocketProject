using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curriculum : MonoBehaviour
{
    private static Curriculum _instance;

    public static Curriculum Instance
    {
        get
        {
            //Creating instance if instance doesn't exist
            if(_instance == null)
            {
                GameObject cm = new GameObject("Curriculum");
                cm.AddComponent<Curriculum>();
            }

            return _instance;
        }
    }

    public bool[] takeOffPadEnabled { get; private set; }
    public int[] maxSteps { get; private set; }
    public float[] legsImpactFailThreshold { get; private set; }
    public float[] minRocketSpawnXPos { get; private set; }
    public float[] maxRocketSpawnXPos { get; private set; }
    public float[] minRocketSpawnYPos { get; private set; }
    public float[] maxRocketSpawnYPos { get; private set; }
    public float[] minLandingPadXPos { get; private set; }
    public float[] maxLandingPadXPos { get; private set; }
    public float[] trainingZoneXScale { get; private set; }
    public float[] trainingZoneYScale { get; private set; }
    public float[] minBuildingYScale { get; private set; }
    public float[] maxBuildingYScale { get; private set; }
    public float[] minPadToPadDist { get; private set; }

    private void Awake()
    {
        _instance = this;

        takeOffPadEnabled = new bool[24] { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true };
        maxSteps = new int[24] { 800, 800, 800, 800, 800, 800, 800, 800, 900, 1000, 1100, 1200, 1300, 1400, 1500, 1600, 1700, 1800, 1900, 2000, 2100, 2200, 2300, 2400 };
        legsImpactFailThreshold = new float[24] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
        minRocketSpawnXPos = new float[24] { -35, -40, -45, -50, -55, -60, -65, -70, -75, -80, -85, -90, -95, -100, -105, -110, -115, -120, -125, -130, -135, -140, -145, -150 };
        maxRocketSpawnXPos = new float[24] { 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 105, 110, 115, 120, 125, 130, 135, 140, 145, 150 };
        minRocketSpawnYPos = new float[24] { 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f };
        maxRocketSpawnYPos = new float[24] { 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f };
        minLandingPadXPos = new float[24] { -35, -40, -45, -50, -55, -60, -65, -70, -75, -80, -85, -90, -95, -100, -105, -110, -115, -120, -125, -130, -135, -140, -145, -150 };
        maxLandingPadXPos = new float[24] { 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100, 105, 110, 115, 120, 125, 130, 135, 140, 145, 150 };
        trainingZoneXScale = new float[24] { 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 260, 270, 280, 290, 300, 310, 320 };
        trainingZoneYScale = new float[24] { 100, 100, 100, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 260, 270, 280, 290, 300 };
        minBuildingYScale = new float[24] { 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f };
        maxBuildingYScale = new float[24] { 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35 };
        minPadToPadDist = new float[24] { 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 };

        takeOffPadEnabled = new bool[47] { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true };
        maxSteps = new int[47] { 800, 800, 800, 800, 800, 800, 800, 800, 800, 800, 800, 800, 800, 800, 800, 900, 900, 1000, 1000, 1100, 1100, 1200, 1200, 1300, 1300, 1400, 1400, 1500, 1500, 1600, 1600, 1700, 1700, 1800, 1800, 1900, 1900, 2000, 2000, 2100, 2100, 2200, 2200, 2300, 2300, 2400, 2400 };
        legsImpactFailThreshold = new float[47] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2};
        minRocketSpawnXPos = new float[47] { -35, -37.5f, -40, -42.5f, -45, -47.5f, -50, -52.5f, -55, -57.5f, -60, -62.5f, -65, -67.5f, -70, -72.5f, -75, -77.5f, -80, -82.5f, -85, -87.5f, -90, -92.5f, -95, -97.5f, -100, -102.5f, -105, -107.5f, -110, -112.5f, -115, -117.5f, -120, -122.5f, -125, -127.5f, -130, -132.5f, -135, -137.5f, -140, -142.5f, -145, -147.5f, -150 };
        maxRocketSpawnXPos = new float[47] { 35, 37.5f, 40, 42.5f, 45, 47.5f, 50, 52.5f, 55, 57.5f, 60, 62.5f, 65, 67.5f, 70, 72.5f, 75, 77.5f, 80, 82.5f, 85, 87.5f, 90, 92.5f, 95, 97.5f, 100, 102.5f, 105, 107.5f, 110, 112.5f, 115, 117.5f, 120, 122.5f, 125, 127.5f, 130, 132.5f, 135, 137.5f, 140, 142.5f, 145, 147.5f, 150 };
        minRocketSpawnYPos = new float[47] { 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f };
        maxRocketSpawnYPos = new float[47] { 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f, 1.9f };
        minLandingPadXPos = new float[47] { -35, -37.5f, -40, -42.5f, -45, -47.5f, -50, -52.5f, -55, -57.5f, -60, -62.5f, -65, -67.5f, -70, -72.5f, -75, -77.5f, -80, -82.5f, -85, -87.5f, -90, -92.5f, -95, -97.5f, -100, -102.5f, -105, -107.5f, -110, -112.5f, -115, -117.5f, -120, -122.5f, -125, -127.5f, -130, -132.5f, -135, -137.5f, -140, -142.5f, -145, -147.5f, -150 };
        maxLandingPadXPos = new float[47] { 35, 37.5f, 40, 42.5f, 45, 47.5f, 50, 52.5f, 55, 57.5f, 60, 62.5f, 65, 67.5f, 70, 72.5f, 75, 77.5f, 80, 82.5f, 85, 87.5f, 90, 92.5f, 95, 97.5f, 100, 102.5f, 105, 107.5f, 110, 112.5f, 115, 117.5f, 120, 122.5f, 125, 127.5f, 130, 132.5f, 135, 137.5f, 140, 142.5f, 145, 147.5f, 150 };
        trainingZoneXScale = new float[47] { 90, 90, 100, 100, 110, 110, 120, 120, 130, 130, 140, 140, 150, 150, 160, 160, 170, 170, 180, 180, 190, 190, 200, 200, 210, 210, 220, 200, 230, 230, 240, 240, 250, 250, 260, 260, 270, 270, 280, 280, 290, 290, 300, 300, 310, 310, 320 };
        trainingZoneYScale = new float[47] { 100, 100, 100, 100, 100, 100, 100, 100, 110, 110, 120, 120, 130, 130, 140, 140, 150, 150, 160, 160, 170, 170, 180, 180, 190, 190, 200, 200, 210, 210, 220, 220, 230, 230, 240, 240, 250, 250, 260, 260, 270, 270, 280, 280, 290, 290, 300 };
        minBuildingYScale = new float[47] { 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f, 2.5f };
        maxBuildingYScale = new float[47] { 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35, 35 };
        minPadToPadDist = new float[47] { 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 };
    }
}
