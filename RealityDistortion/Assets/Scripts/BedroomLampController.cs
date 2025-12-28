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
    private bool canChange = true; // permite schimbarea la ieșirea din view

    void Start()
    {
        playerCamera = Camera.main;
        ApplyLight();
    }

    void Update()
    {
        if (IsClearlyOutOfView())
        {
            if (canChange)
            {
                outOfViewTimer += Time.deltaTime;

                if (outOfViewTimer >= notInViewDelay)
                {
                    // schimbăm starea
                    isWarm = !isWarm;
                    canChange = false;   // blocăm până revine în view
                }
            }
        }
        else
        {
            // când revine în view, aplicăm culoarea și resetăm timer
            ApplyLight();
            outOfViewTimer = 0f;
            canChange = true;  // permite următoarea schimbare
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
}