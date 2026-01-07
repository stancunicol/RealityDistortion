using UnityEngine;

public class DoorClose : MonoBehaviour
{
    public Transform player;
    public float closeDistance = 3f;
    public Vector3 closedRotation;
    private Vector3 openRotation;

    private bool isClosed = false;

    void Start()
    {
        openRotation = transform.eulerAngles;
    }

    void Update()
    {
        if (isClosed) return;

        float dist = Vector3.Distance(player.position, transform.position);
        if (dist <= closeDistance)
        {
            CloseDoor();
        }
    }

    void CloseDoor()
    {
        transform.eulerAngles = closedRotation;
        isClosed = true;
    }
    
    public void ResetAnomaly()
    {
        transform.eulerAngles = openRotation;
        isClosed = false;
    }
}
