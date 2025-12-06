using UnityEngine;
using UnityEngine.Video;

public class TVSwitch : MonoBehaviour
{
    [SerializeField] private Material screenOff;
    [SerializeField] private Material screenVideo;

    private VideoPlayer videoPlayer;
    private MeshRenderer meshRenderer;
    private bool isOn = false;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        meshRenderer = GetComponent<MeshRenderer>();
        Material[] mats = meshRenderer.materials;
        mats[2] = screenOff;
        meshRenderer.materials = mats;
        if (videoPlayer != null)
            videoPlayer.Stop();
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
    }
}
