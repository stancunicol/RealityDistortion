using UnityEngine;

public class ClockPainting : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material anomalyMaterial;
    [SerializeField] private GameObject realClock;
    [SerializeField] private AudioSource tickSound;
    [SerializeField] private float horizontalThreshold = 40f;
    [SerializeField] private float verticalThreshold = 30f;
    [SerializeField] private float delayAfterLook = 5f;
    [SerializeField] private float maxDistance = 5f;

    private MeshRenderer meshRenderer;
    private bool timerStarted = false;
    private float timer = 0f;
    private bool anomalyActivated = false;

    void Start()
    {
        meshRenderer = transform.Find("Plane").GetComponent<MeshRenderer>();
        meshRenderer.material = normalMaterial;
        if (tickSound != null) tickSound.Stop();
        if (realClock != null) realClock.SetActive(false);
    }

    void Update()
    {
        if (!playerCamera || !playerTransform || anomalyActivated) return;

        float distanceToPainting = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPainting > maxDistance)
        {
            timerStarted = false;
            timer = 0f;
            return;
        }

        Vector3 directionToPainting = (transform.position - playerCamera.position).normalized;
        Vector3 flatForward = new Vector3(playerCamera.forward.x, 0, playerCamera.forward.z).normalized;
        Vector3 flatToPainting = new Vector3(directionToPainting.x, 0, directionToPainting.z).normalized;

        float horizontalAngle = Vector3.Angle(flatForward, flatToPainting);
        float verticalAngle = Mathf.Abs(Vector3.Angle(playerCamera.forward, directionToPainting) - horizontalAngle);

        bool lookingDirectly = horizontalAngle < horizontalThreshold && verticalAngle < verticalThreshold;

        if (lookingDirectly)
        {
            if (!timerStarted)
            {
                timerStarted = true;
                timer = 0f;
            }

            timer += Time.deltaTime;
            if (timer >= delayAfterLook)
            {
                meshRenderer.material = anomalyMaterial;
                if (realClock != null) realClock.SetActive(true);
                if (tickSound != null) tickSound.Play();
                anomalyActivated = true;
            }
        }
        else
        {
            timerStarted = false;
            timer = 0f;
        }
    }
}
