using UnityEngine;

public class Doll : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private GameObject normalDoll;
    [SerializeField] private GameObject creepyDoll;
    [SerializeField] private float viewAngle = 60f;

    [SerializeField] private Transform tableRight;
    [SerializeField] private Transform tableLeft;
    [SerializeField] private AudioSource footstep;
    [SerializeField] private float triggerDistance = 10f;
    [SerializeField] private float footstepDuration = 2f;

    private bool hasMoved = false;

    void Update()
    {
        HandleViewAnomaly();
        HandlePositionAnomaly();
    }

    private void HandleViewAnomaly()
    {
        Vector3 directionToDoll = (transform.position - playerCamera.position).normalized;
        float angle = Vector3.Angle(playerCamera.forward, directionToDoll);

        if (angle < viewAngle)
        {
            normalDoll.SetActive(true);
            creepyDoll.SetActive(false);
        }
        else
        {
            normalDoll.SetActive(false);
            creepyDoll.SetActive(true);
        }
    }

    private void HandlePositionAnomaly()
    {
        if (hasMoved)
            return;

        float distance = Vector3.Distance(playerCamera.position, tableRight.position);
        Vector3 directionToDoll = (transform.position - playerCamera.position).normalized;
        float angle = Vector3.Angle(playerCamera.forward, directionToDoll);

        if (distance > triggerDistance && angle > viewAngle / 2)
        {
            MoveDoll();
            hasMoved = true;
        }
    }

    private void MoveDoll()
    {
        transform.parent = tableLeft;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0, 180f, 0);

        if (footstep != null)
            StartCoroutine(PlayFootstepLimited(footstepDuration));
    }

    private System.Collections.IEnumerator PlayFootstepLimited(float duration)
    {
        footstep.Play();
        yield return new WaitForSeconds(duration);
        footstep.Stop();
    }
    
    public void ResetAnomaly()
    {
        hasMoved = false;
        transform.parent = tableRight;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        if (normalDoll != null) normalDoll.SetActive(true);
        if (creepyDoll != null) creepyDoll.SetActive(false);
        if (footstep != null && footstep.isPlaying) footstep.Stop();
    }
}
