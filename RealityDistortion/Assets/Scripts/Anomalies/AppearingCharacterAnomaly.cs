using UnityEngine;

public class AppearingCharacterAnomaly : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float activationDistance = 10f;
    [SerializeField] private float detectionRange = 40f;

    [Header("View Angles")]
    [SerializeField] private float lookAtAngleEnter = 20f; // grade
    [SerializeField] private float lookAtAngleExit = 35f;  // grade (mai mare!)

    [Header("Appearances")]
    [SerializeField] private int maxAppearances = 2;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string lookAnimBool = "IsLookedAt";

    private bool activated = false;
    private bool isVisible = false;
    private bool isLookedAt = false;
    private int appearanceCount = 0;

    private Renderer[] renderers;

    private void Start()
    {
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;

        renderers = GetComponentsInChildren<Renderer>(true);
        ForceHide();

        Debug.Log("[Anomaly] Ready.");
    }

    private void Update()
    {
        float distance = Vector3.Distance(playerCamera.position, transform.position);

        // ACTIVARE
        if (!activated)
        {
            if (distance <= activationDistance)
            {
                activated = true;
                Debug.Log("[Anomaly] ACTIVATED");
            }
            else return;
        }

        if (appearanceCount >= maxAppearances)
            return;

        bool lookingNow = IsLookingAtMe(distance);

        // INTRARE PRIVIRE
        if (lookingNow && !isLookedAt)
        {
            Debug.Log("[Anomaly] START LOOK");
            isLookedAt = true;
            Show();
        }
        // IESIRE PRIVIRE
        else if (!lookingNow && isLookedAt)
        {
            Debug.Log("[Anomaly] STOP LOOK");
            isLookedAt = false;
            Hide();
        }

        if (animator != null)
            animator.SetBool(lookAnimBool, isLookedAt);
    }

    private bool IsLookingAtMe(float distance)
    {
        if (distance > detectionRange)
            return false;

        Vector3 toMe = (transform.position - playerCamera.position).normalized;
        float angle = Vector3.Angle(playerCamera.forward, toMe);

        if (!isLookedAt)
            return angle <= lookAtAngleEnter;
        else
            return angle <= lookAtAngleExit;
    }

    private void Show()
    {
        if (isVisible) return;

        foreach (var r in renderers)
            r.enabled = true;

        isVisible = true;
        appearanceCount++;

        Debug.Log($"[Anomaly] SHOW ({appearanceCount}/{maxAppearances})");
    }

    private void Hide()
    {
        if (!isVisible) return;

        foreach (var r in renderers)
            r.enabled = false;

        isVisible = false;

        Debug.Log("[Anomaly] HIDE");
    }

    private void ForceHide()
    {
        foreach (var r in renderers)
            r.enabled = false;

        isVisible = false;
    }
}
