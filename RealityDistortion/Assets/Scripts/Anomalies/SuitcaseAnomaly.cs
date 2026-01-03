using UnityEngine;

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

        if (timer >= nextChange && !IsInView())
        {
            MoveSuitcase();
            timer = 0f;
            nextChange = Random.Range(minTime, maxTime);
        }
    }

    bool IsInView()
    {
        Vector3 vp = playerCamera.WorldToViewportPoint(transform.position);
        return vp.z > 0 && vp.x > 0 && vp.x < 1 && vp.y > 0 && vp.y < 1;
    }

    void MoveSuitcase()
    {
        int index = Random.Range(0, positions.Length);
        transform.position = positions[index].position;
    }
}
