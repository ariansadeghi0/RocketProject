using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaySpawner : MonoBehaviour
{
    [SerializeField] private GameObject linePrefab;

    [SerializeField] private int numberOfRays;
    [SerializeField] private int maxDistance;
    [SerializeField] private float rayEmissionPointOffset;

    [SerializeField] private float lineTransparency;
    [SerializeField] private LayerMask collidableLayers;

    private GameObject[] lines;
    private LineRenderer[] lineRenderers;
    private float initialLineWidth;
    private float initialCamOrthographicSize;

    private void Awake()
    {
        lines = new GameObject[numberOfRays];
        lineRenderers = new LineRenderer[numberOfRays];

        for (int i = 0; i < numberOfRays; i++)
        {
            lines [i] = Instantiate(linePrefab, Vector3.zero, Quaternion.Euler(0, 0, i * (360 / numberOfRays) - 90), this.transform);
            lineRenderers[i] = lines[i].GetComponent<LineRenderer>();
            lineRenderers[i].positionCount = 2;
        }
        initialLineWidth = lineRenderers[0].startWidth;
        initialCamOrthographicSize = Camera.main.orthographicSize;
    }
    private void Update()
    {
        for (int i = 0; i < numberOfRays; i++)
        {
            float dist;
            float lineRotation = lines[i].transform.rotation.eulerAngles.z;

            if (lineRotation > 180)
            {
                lineRotation -= 360;
            }

            float x = Mathf.Cos((lineRotation * Mathf.Deg2Rad));
            float y = Mathf.Sin((lineRotation * Mathf.Deg2Rad));
            
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), (Vector2.zero - new Vector2(x, y)), maxDistance, collidableLayers);

            Vector2 selfPos = new Vector2(transform.position.x, transform.position.y);
            if (hit.collider != null)
            {
                lineRenderers[i].SetPosition(0, new Vector2(-x * rayEmissionPointOffset + transform.position.x, -y * rayEmissionPointOffset + transform.position.y));
                lineRenderers[i].SetPosition(1, hit.point);

                dist = Vector2.Distance(selfPos, hit.point);

                lineRenderers[i].startColor = new Color(1, dist / maxDistance, dist / maxDistance, lineTransparency);
                lineRenderers[i].endColor = new Color(1, dist / maxDistance, dist / maxDistance, lineTransparency);
            }
            else
            {
                lineRenderers[i].SetPosition(0, new Vector2(-x * rayEmissionPointOffset + transform.position.x, -y * rayEmissionPointOffset + transform.position.y));
                lineRenderers[i].SetPosition(1, new Vector2(-x * maxDistance + transform.position.x, -y * maxDistance + transform.position.y));

                lineRenderers[i].startColor = new Color(1, 1, 1, lineTransparency);
                lineRenderers[i].endColor = new Color(1, 1, 1, lineTransparency);
            }

            lineRenderers[i].startWidth = initialLineWidth * (Camera.main.orthographicSize / initialCamOrthographicSize);
            Debug.Log(Camera.main.orthographicSize / initialCamOrthographicSize);
        }
    }
}
