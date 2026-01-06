using UnityEngine;

public class WomanSculpture : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform playerBody;
    [SerializeField] private AudioSource rotationAudio;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float lookThreshold = 40f;
    [SerializeField] private float stopThreshold = 5f;

    private void Update()
    {
        if (!playerCamera || !playerBody) return;

        Vector3 toPlayer = playerBody.position - transform.position;
        Vector3 flatToPlayer = new Vector3(toPlayer.x, 0f, toPlayer.z);
        if (flatToPlayer.sqrMagnitude < 0.0001f) flatToPlayer = Vector3.forward;
        flatToPlayer.Normalize();

        Vector3 cameraFlatForward = new Vector3(playerCamera.forward.x, 0f, playerCamera.forward.z);
        if (cameraFlatForward.sqrMagnitude < 0.0001f) cameraFlatForward = Vector3.forward;
        cameraFlatForward.Normalize();

        float horizontalAngle = Vector3.Angle(cameraFlatForward, flatToPlayer);
        bool playerLooking = horizontalAngle > lookThreshold;

        Vector3 sculptureForward = transform.TransformDirection(Vector3.forward);
        Vector3 flatSculptureForward = new Vector3(sculptureForward.x, 0f, sculptureForward.z);
        if (flatSculptureForward.sqrMagnitude < 0.0001f) flatSculptureForward = Vector3.forward;
        flatSculptureForward.Normalize();

        float facingAngle = Vector3.Angle(flatSculptureForward, flatToPlayer);
        bool sculptureFacingPlayer = facingAngle < stopThreshold;
        Debug.Log(facingAngle);
        if (!playerLooking && !sculptureFacingPlayer)
        {
            float targetY = Mathf.Atan2(flatToPlayer.x, flatToPlayer.z) * Mathf.Rad2Deg + 180f;
            float currentY = transform.eulerAngles.y;
            float deltaY = Mathf.DeltaAngle(currentY, targetY);

            float newY = Mathf.MoveTowardsAngle(currentY, targetY, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(-90f, newY, 0f);

            if (rotationAudio)
            {
                if (Mathf.Abs(deltaY) > 0.1f && !rotationAudio.isPlaying)
                    rotationAudio.Play();
                else if (Mathf.Abs(deltaY) <= 0.1f && rotationAudio.isPlaying)
                    rotationAudio.Stop();
            }
        }
        else
        {
            if (rotationAudio && rotationAudio.isPlaying)
                rotationAudio.Stop();
        }
    }
}
