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

    [Header("Video / Settings")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private float channelInterval = 1.0f;

    [Header("Pause Between Channels")]
    [SerializeField] private float pauseDuration = 0.3f;

    [Header("Audio")]
    [SerializeField] private AudioSource staticAudio;

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

    void Start()
    {
        armAnimator = arm != null ? arm.GetComponent<Animator>() : null;
        meshRenderer = GetComponent<MeshRenderer>();

        Material[] mats = meshRenderer.materials;
        mats[2] = screenOff;
        meshRenderer.materials = mats;

        if (videoPlayer != null)
            videoPlayer.Stop();

        if (arm != null)
            arm.SetActive(false);

        channelMaterials = new Material[] { screenStatic1, screenStatic2, screenCreepy };
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
            {
                ToggleTV();
            }
        }

        if (isOn && !isCreepyActive)
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

                    if (currentChannel >= channelMaterials.Length - 1)
                    {
                        ActivateCreepyChannel();
                    }
                    else
                    {
                        lastChannelMaterial = channelMaterials[currentChannel];
                        SetTVMaterial(screenOff);
                        if (staticAudio != null && staticAudio.isPlaying)
                            staticAudio.Stop();
                        isPausing = true;
                    }
                }
            }
        }
    }

    void ToggleTV()
    {
        isOn = !isOn;

        SetTVMaterial(isOn ? screenStatic1 : screenOff);

        if (videoPlayer != null)
        {
            if (isOn)
                videoPlayer.Play();
            else
                videoPlayer.Stop();
        }

        if (!isOn && arm != null)
            arm.SetActive(false);

        currentChannel = 0;
        timer = 0f;
        isCreepyActive = false;
        isPausing = false;
        pauseTimer = 0f;

        UpdateStaticAudio();
    }

    void SetTVMaterial(Material mat)
    {
        Material[] mats = meshRenderer.materials;
        mats[2] = mat;
        meshRenderer.materials = mats;
    }

    void ActivateCreepyChannel()
    {
        isCreepyActive = true;
        SetTVMaterial(screenCreepy);

        if (arm != null)
        {
            arm.SetActive(true);
            if (armAnimator != null && reachClip != null)
                armAnimator.Play(reachClip.name, -1, 0f);
        }

        if (staticAudio != null && staticAudio.isPlaying)
            staticAudio.Stop();
    }

    void UpdateStaticAudio()
    {
        if (isOn && currentChannel < channelMaterials.Length - 1)
        {
            if (staticAudio != null && !staticAudio.isPlaying)
                staticAudio.Play();
        }
        else
        {
            if (staticAudio != null && staticAudio.isPlaying)
                staticAudio.Stop();
        }
    }
}
