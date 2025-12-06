using UnityEngine;
using UnityEngine.Video;

public class TVSwitch : MonoBehaviour
{
    [SerializeField] private Material screenOff;
    [SerializeField] private Material screenVideo;
    [SerializeField] private GameObject arm;
    [SerializeField] private AnimationClip reachClip;

    private Animator armAnimator;
    private VideoPlayer videoPlayer;
    private MeshRenderer meshRenderer;
    private bool isOn = false;

    void Start()
    {
        armAnimator = arm != null ? arm.GetComponent<Animator>() : null;
        videoPlayer = GetComponent<VideoPlayer>();
        meshRenderer = GetComponent<MeshRenderer>();

        Material[] mats = meshRenderer.materials;
        mats[2] = screenOff;
        meshRenderer.materials = mats;

        if (videoPlayer != null)
            videoPlayer.Stop();

        if (arm != null)
            arm.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    ToggleTV();
                }
            }
        }
    }

    void ToggleTV()
    {
        isOn = !isOn;

        Material[] mats = meshRenderer.materials;
        mats[2] = isOn ? screenVideo : screenOff;
        meshRenderer.materials = mats;

        if (videoPlayer != null)
        {
            if (isOn)
                videoPlayer.Play();
            else
                videoPlayer.Stop();
        }

        if (arm != null)
        {
            if (isOn)
            {
                arm.SetActive(true);
                if (armAnimator != null && reachClip != null)
                {
                    armAnimator.Play(reachClip.name, -1, 0f);
                }
            }
            else
            {
                arm.SetActive(false);
            }
        }
    }
}
