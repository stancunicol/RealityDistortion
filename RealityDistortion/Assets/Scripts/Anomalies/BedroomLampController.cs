using UnityEngine;

public class BedroomLampController : MonoBehaviour
{
    public Light lampLight;

    public Color warmColor = new Color(1f, 0.78f, 0.6f);
    public Color coldColor = new Color(0.8f, 0.9f, 1f);

    [Header("Tuning")]
    public float notInViewDelay = 0.4f;   // cât timp trebuie să NU fie privită
    public float centerDeadZone = 0.25f;  // zona centrală unde NU declanșează

    private Camera playerCamera;
    private bool isWarm = true;
    private float outOfViewTimer = 0f;
    private bool canChange = true; 

    void Start()
    {
        playerCamera = Camera.main;
        ApplyLight();
    }

    void Update()
    {
        if (playerCamera == null) return;
        
        if (IsClearlyOutOfView())
        {
            if (canChange)
            {
                outOfViewTimer += Time.deltaTime;

                if (outOfViewTimer >= notInViewDelay)
                {
                    isWarm = !isWarm;
                    ApplyLight();  
                    canChange = false; 
                }
            }
        }
        else
        {
            outOfViewTimer = 0f;
            canChange = true;  
        }
    }

    bool IsClearlyOutOfView()
    {
        Vector3 vp = playerCamera.WorldToViewportPoint(transform.position);

        if (vp.z <= 0) return true;

        bool inCenter =
            vp.x > centerDeadZone && vp.x < 1f - centerDeadZone &&
            vp.y > centerDeadZone && vp.y < 1f - centerDeadZone;

        return !inCenter;
    }

    void ApplyLight()
    {
        lampLight.enabled = true;
        lampLight.color = isWarm ? warmColor : coldColor;
    }
    
    public void ResetAnomaly()
    {
        isWarm = true;
        outOfViewTimer = 0f;
        canChange = true;
        if (lampLight != null)
        {
            lampLight.enabled = true;
            lampLight.color = warmColor;
        }
    }
}