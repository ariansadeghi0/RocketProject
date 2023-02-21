using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour
{
    [SerializeField] Transform trainingEnvs;
    [SerializeField] GameObject environmentPrefab;
    [SerializeField] [Range(0, 500)]int envsToSpawn;
    [SerializeField] float yDistanceBetweenEnvs;

    private void Awake()
    {
        for (int i = 0; i < envsToSpawn; i++)
        {
            Instantiate(environmentPrefab, new Vector3(0, (i * yDistanceBetweenEnvs) + yDistanceBetweenEnvs), environmentPrefab.transform.rotation, trainingEnvs);
        }
    }
}
