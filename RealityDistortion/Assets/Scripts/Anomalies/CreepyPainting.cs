using UnityEngine;

public class CreepyPainting : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material creepyMaterial;
    [SerializeField] private AudioSource screamAudio;
    [SerializeField] private float horizontalThreshold = 40f;
    [SerializeField] private float verticalThreshold = 30f;

    private MeshRenderer meshRenderer;

    void Awake()
    {
        meshRenderer = transform.Find("Plane").GetComponent<MeshRenderer>();
    }

    void Start()
    {
        if (meshRenderer != null && normalMaterial != null)
            meshRenderer.material = normalMaterial;

        if (screamAudio != null)
            screamAudio.Stop();
    }

    void Update()
    {
        if (!playerCamera || meshRenderer == null) return;

        Vector3 directionToPainting = (transform.position - playerCamera.position).normalized;

        Vector3 flatForward = new Vector3(playerCamera.forward.x, 0, playerCamera.forward.z);
        Vector3 flatToPainting = new Vector3(directionToPainting.x, 0, directionToPainting.z);
        float horizontalAngle = Vector3.Angle(flatForward, flatToPainting);

        float verticalAngle = Mathf.Abs(Vector3.Angle(playerCamera.forward, directionToPainting) - horizontalAngle);

        bool lookingDirectly = horizontalAngle < horizontalThreshold && verticalAngle < verticalThreshold;

        if (lookingDirectly)
        {
            if (normalMaterial != null)
                meshRenderer.material = normalMaterial;
            if (screamAudio != null && screamAudio.isPlaying)
                screamAudio.Stop();
        }
        else
        {
            if (creepyMaterial != null)
                meshRenderer.material = creepyMaterial;
            if (screamAudio != null && !screamAudio.isPlaying)
                screamAudio.Play();
        }
    }
    
    public void ResetAnomaly()
    {
        if (meshRenderer != null && normalMaterial != null)
            meshRenderer.material = normalMaterial;
        if (screamAudio != null && screamAudio.isPlaying)
            screamAudio.Stop();
    }
}
