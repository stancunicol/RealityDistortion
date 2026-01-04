using UnityEngine;

public class Doll : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private GameObject normalDoll;
    [SerializeField] private GameObject creepyDoll;
    [SerializeField] private float viewAngle = 60f;

    void Update()
    {
        Vector3 directionToDoll = (transform.position - playerCamera.position).normalized;
        float angle = Vector3.Angle(playerCamera.forward, directionToDoll);

        if (angle < viewAngle)
        {
            normalDoll.SetActive(false);
            creepyDoll.SetActive(true);
        }
        else
        {
            normalDoll.SetActive(true);
            creepyDoll.SetActive(false);
        }
    }
}
