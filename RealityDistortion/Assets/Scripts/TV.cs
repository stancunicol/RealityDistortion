using UnityEngine;
using UnityEngine.Video;

public class TV : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material screenOff;
    [SerializeField] private Material screenStatic1;
    [SerializeField] private Material screenStatic2;
    [SerializeField] private Material screenCreepy;
    [SerializeField] private Material screenBlack;

    [Header("Settings")]
    [SerializeField] private float pauseDuration = 1f;
    [SerializeField] private AudioSource staticAudio;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private float horizontalThreshold = 35f;
    [SerializeField] private Transform playerCamera;

    private MeshRenderer meshRenderer;
    private bool isOn = false;
    private Material[] channelMaterials;
    private int currentChannel = 0;
    private bool isCreepyStage = false;
    private Material lastChannelMaterial;
    private bool isPausing = false;
    private float pauseTimer = 0f;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = screenOff;
        channelMaterials = new Material[] { screenStatic1, screenStatic2 };
        lastChannelMaterial = screenStatic1;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
            {
                if (!isOn) ToggleTV();
                else NextChannel();
            }
        }

        if (!isOn) return;

        if (isPausing)
        {
            pauseTimer += Time.deltaTime;
            SetTVMaterial(screenBlack);
            if (pauseTimer >= pauseDuration)
            {
                isPausing = false;
                pauseTimer = 0f;
                SetTVMaterial(lastChannelMaterial);
                UpdateStaticAudio();
            }
            return;
        }

        if (isCreepyStage) HandleCreepyView();
        else SetTVMaterial(lastChannelMaterial);
    }

    void ToggleTV()
    {
        isOn = true;
        currentChannel = 0;
        isCreepyStage = false;
        isPausing = false;
        pauseTimer = 0f;
        SetTVMaterial(screenStatic1);
        UpdateStaticAudio();
        if (videoPlayer != null) videoPlayer.enabled = true;
    }

    void NextChannel()
    {
        if (isPausing || isCreepyStage) return;
        currentChannel++;
        if (currentChannel >= channelMaterials.Length) StartCreepyStage();
        else
        {
            lastChannelMaterial = channelMaterials[currentChannel];
            isPausing = true;
            pauseTimer = 0f;
            UpdateStaticAudio();
        }
    }

    void StartCreepyStage()
    {
        isCreepyStage = true;
        if (videoPlayer != null) videoPlayer.enabled = false;
        if (staticAudio != null && staticAudio.isPlaying) staticAudio.Stop();
    }

    void HandleCreepyView()
    {
        if (!playerCamera) return;
        Vector3 directionToTV = (transform.position - playerCamera.position).normalized;
        Vector3 flatForward = new Vector3(playerCamera.forward.x, 0, playerCamera.forward.z);
        Vector3 flatToTV = new Vector3(directionToTV.x, 0, directionToTV.z);
        float horizontalAngle = Vector3.Angle(flatForward, flatToTV);
        bool lookingDirectly = horizontalAngle < horizontalThreshold;
        SetTVMaterial(lookingDirectly ? screenBlack : screenCreepy, true);
        if (videoPlayer != null) videoPlayer.enabled = false;
        if (staticAudio != null && staticAudio.isPlaying) staticAudio.Stop();
    }

    void SetTVMaterial(Material mat, bool fullUV = false)
    {
        Material[] mats = meshRenderer.materials;
        Material instMat = new Material(mat);
        instMat.mainTextureScale = Vector2.one;
        instMat.mainTextureOffset = Vector2.zero;
        mats[2] = instMat;
        meshRenderer.materials = mats;
    }

    void UpdateStaticAudio()
    {
        if (isOn && !isCreepyStage && !isPausing)
        {
            if (staticAudio != null && !staticAudio.isPlaying) staticAudio.Play();
        }
        else
        {
            if (staticAudio != null && staticAudio.isPlaying) staticAudio.Stop();
        }
    }
}
