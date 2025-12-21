using UnityEngine;
using UnityEngine.Video;

public class TVSwitch : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material screenOff;
    [SerializeField] private Material screenStatic1;
    [SerializeField] private Material screenStatic2;
    [SerializeField] private Material screenCreepy;

    [Header("Arm")]
    [SerializeField] private GameObject arm;
    [SerializeField] private AnimationClip reachClip;

    [Header("Settings")]
    [SerializeField] private float channelInterval = 1.0f;
    [SerializeField] private float pauseDuration = 0.3f;
    [SerializeField] private AudioSource staticAudio;
    [SerializeField] private float creepyDelay = 1.0f;
    [SerializeField] private float flickerSpeed = 0.15f;
    [SerializeField] private VideoPlayer videoPlayer;

    private Animator armAnimator;
    private MeshRenderer meshRenderer;
    private bool isOn = false;

    private Material[] channelMaterials;
    private int currentChannel = 0;
    private float timer = 0f;
    private bool isCreepyActive = false;

    private bool isPausing = false;
    private float pauseTimer = 0f;
    private Material lastChannelMaterial;

    private float creepyTimer = 0f;
    private bool flickerActive = false;
    private float flickerTimer = 0f;

    void Start()
    {
        armAnimator = arm != null ? arm.GetComponent<Animator>() : null;
        meshRenderer = GetComponent<MeshRenderer>();
        Material[] mats = meshRenderer.materials;
        mats[2] = screenOff;
        meshRenderer.materials = mats;
        if (arm != null) arm.SetActive(false);
        channelMaterials = new Material[] { screenStatic1, screenStatic2 };
        if (videoPlayer != null) videoPlayer.enabled = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
                ToggleTV();
        }

        if (isOn && !isCreepyActive)
            HandleChannelSwitching();

        if (flickerActive)
            HandleFlicker();

        if (isCreepyActive)
            HandleCreepyDelay();
    }

    void HandleChannelSwitching()
    {
        if (isPausing)
        {
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= pauseDuration)
            {
                isPausing = false;
                pauseTimer = 0f;
                SetTVMaterial(lastChannelMaterial);
                UpdateStaticAudio();
            }
        }
        else
        {
            timer += Time.deltaTime;
            if (timer >= channelInterval)
            {
                timer = 0f;
                currentChannel++;
                if (currentChannel >= channelMaterials.Length)
                    StartCreepyStage();
                else
                {
                    lastChannelMaterial = channelMaterials[currentChannel];
                    SetTVMaterial(screenOff);
                    if (staticAudio != null && staticAudio.isPlaying) staticAudio.Stop();
                    if (videoPlayer != null) videoPlayer.enabled = true;
                    isPausing = true;
                }
            }
        }
    }

    void StartCreepyStage()
    {
        isCreepyActive = true;
        creepyTimer = 0f;
        flickerActive = true;
        if (videoPlayer != null) videoPlayer.enabled = false;
        if (staticAudio != null && staticAudio.isPlaying) staticAudio.Stop();
    }

    void HandleCreepyDelay()
    {
        creepyTimer += Time.deltaTime;
        if (creepyTimer >= creepyDelay)
        {
            flickerActive = false;
            SetTVMaterial(screenCreepy, true);
            ActivateArmCreepy();
            isCreepyActive = false;
        }
    }

    void HandleFlicker()
    {
        flickerTimer += Time.deltaTime;
        if (flickerTimer >= flickerSpeed)
        {
            flickerTimer = 0f;
            Material current = meshRenderer.materials[2].name.Contains(screenOff.name) ? lastChannelMaterial : screenOff;
            if (meshRenderer.materials[2].name.Contains(screenCreepy.name))
                current = screenCreepy;
            SetTVMaterial(current, current == screenCreepy);
        }
    }

    void ActivateArmCreepy()
    {
        if (arm != null)
        {
            arm.SetActive(true);
            if (armAnimator != null && reachClip != null)
                armAnimator.Play(reachClip.name, -1, 0f);
        }
    }

    void ToggleTV()
    {
        isOn = !isOn;
        SetTVMaterial(isOn ? screenStatic1 : screenOff);
        if (videoPlayer != null) videoPlayer.enabled = isOn;
        if (!isOn && arm != null) arm.SetActive(false);
        currentChannel = 0;
        timer = 0f;
        isCreepyActive = false;
        creepyTimer = 0f;
        isPausing = false;
        pauseTimer = 0f;
        flickerActive = false;
        UpdateStaticAudio();
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
        if (isOn && !isCreepyActive)
        {
            if (staticAudio != null && !staticAudio.isPlaying) staticAudio.Play();
        }
        else
        {
            if (staticAudio != null && staticAudio.isPlaying) staticAudio.Stop();
        }
    }
}
