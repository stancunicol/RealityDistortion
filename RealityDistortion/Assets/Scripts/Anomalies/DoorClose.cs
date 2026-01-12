using UnityEngine;

public class DoorClose : MonoBehaviour
{
    public Transform player;
    public float closeDistance = 3f;
    public GameObject door7;          // Door_7
    public GameObject door7Locked;    // Door_7_Locked

    private bool isClosed = false;

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
        if (door7 != null)
            door7.SetActive(false);
        
        if (door7Locked != null)
            door7Locked.SetActive(true);
        
        isClosed = true;
    }

    public void ResetAnomaly()
    {
        if (door7 != null)
            door7.SetActive(true);
        
        if (door7Locked != null)
            door7Locked.SetActive(false);
        
        isClosed = false;
    }
}
