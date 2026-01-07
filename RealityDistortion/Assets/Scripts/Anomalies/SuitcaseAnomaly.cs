using UnityEngine;
using System.Collections.Generic;

public class SuitcaseAnomaly : MonoBehaviour
{
    public Transform[] positions;  
    public float minTime = 3f;    
    public float maxTime = 8f;     

    private Camera playerCamera;
    private float timer;
    private float nextChange;

    void Start()
    {
        playerCamera = Camera.main;
        nextChange = Random.Range(minTime, maxTime);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= nextChange)
        {
            if (!IsInView())
            {
                MoveSuitcaseToInvisiblePosition();
            }

            timer = 0f;
            nextChange = Random.Range(minTime, maxTime);
        }
    }

    bool IsInView()
    {
        Vector3 vp = playerCamera.WorldToViewportPoint(transform.position);
        return vp.z > 0 && vp.x > 0 && vp.x < 1 && vp.y > 0 && vp.y < 1;
    }

    void MoveSuitcaseToInvisiblePosition()
    {
        List<Transform> invisiblePositions = new List<Transform>();

        foreach (Transform pos in positions)
        {
            Vector3 vp = playerCamera.WorldToViewportPoint(pos.position);

            if (vp.z < 0 || vp.x < 0 || vp.x > 1 || vp.y < 0 || vp.y > 1)
            {
                invisiblePositions.Add(pos);
            }
        }

        if (invisiblePositions.Count == 0)
        {
            return;
        }

        int index = Random.Range(0, invisiblePositions.Count);
        transform.position = invisiblePositions[index].position;
    }
    
    public void ResetAnomaly()
    {
        timer = 0f;
        nextChange = Random.Range(minTime, maxTime);
        if (positions != null && positions.Length > 0)
        {
            transform.position = positions[0].position;
        }
    }
}