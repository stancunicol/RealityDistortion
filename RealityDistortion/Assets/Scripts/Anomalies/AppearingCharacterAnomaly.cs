using UnityEngine;

public class AppearingCharacterAnomaly : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float activationDistance = 10f;
    [SerializeField] private float detectionRange = 40f;

    [Header("View Angles")]
    [SerializeField] private float lookAtAngleEnter = 20f;
    [SerializeField] private float lookAtAngleExit = 35f;

    [Header("Look Target Offset")]
    [SerializeField] private Vector3 lookOffset = new Vector3(0f, 1.6f, 0f);

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
    private Collider[] colliders;

    private void Start()
    {
        if (playerCamera == null && Camera.main != null)
            playerCamera = Camera.main.transform;

        if (playerCamera == null)
        {
            enabled = false;
            return;
        }

        renderers = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);

        ForceHide();

    }

    private void Update()
    {
        float distance = Vector3.Distance(playerCamera.position, transform.position);

        if (!activated)
        {
            if (distance <= activationDistance)
            {
                activated = true;
            }
            else return;
        }

        bool lookingNow = IsLookingAtMe(distance);

        if (lookingNow && !isLookedAt)
        {
            isLookedAt = true;
            Show();
        }
        else if (!lookingNow && isLookedAt)
        {
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

        Vector3 lookTarget = transform.position + lookOffset;
        Vector3 toTarget = (lookTarget - playerCamera.position).normalized;

        float angle = Vector3.Angle(playerCamera.forward, toTarget);

        return !isLookedAt
            ? angle <= lookAtAngleEnter
            : angle <= lookAtAngleExit;
    }

    private void Show()
    {
        if (isVisible) return;
        if (appearanceCount >= maxAppearances)
            return;

        foreach (Renderer r in renderers)
            if (r != null) r.enabled = true;

        foreach (Collider c in colliders)
            if (c != null) c.enabled = true;

        isVisible = true;
        appearanceCount++;

    }

    private void Hide()
    {
        if (!isVisible) return;

        foreach (Renderer r in renderers)
            if (r != null) r.enabled = false;

        foreach (Collider c in colliders)
            if (c != null) c.enabled = false;

        isVisible = false;

    }

    private void ForceHide()
    {
        foreach (Renderer r in renderers)
            if (r != null) r.enabled = false;

        foreach (Collider c in colliders)
            if (c != null) c.enabled = false;

        isVisible = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + lookOffset, 0.15f);
    }
#endif
}
