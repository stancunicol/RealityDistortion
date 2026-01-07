using UnityEngine;

public class WaypointRunner : MonoBehaviour
{
    [Header("Waypoints")]
    [Tooltip("List of waypoints the character will follow in order")]
    public Transform[] waypoints;
    
    [Header("Movement Settings")]
    [Tooltip("Running speed of the character")]
    public float runSpeed = 8f;
    [Tooltip("Minimum distance to consider waypoint reached")]
    public float waypointReachDistance = 0.5f;
    [Tooltip("If true, will loop through waypoints")]
    public bool loopWaypoints = true;
    [Tooltip("If true, will start automatically on Start()")]
    public bool startOnAwake = false;
    [Tooltip("If true, character will disappear after completing one full route")]
    public bool disappearAfterComplete = true;
    
    [Header("Animation (Optional)")]
    [Tooltip("Animator for running animation")]
    public Animator animator;
    [Tooltip("Name of the animation parameter for running")]
    public string runAnimationParameter = "IsRunning";
    [Tooltip("Type of animation parameter")]
    public AnimationParameterType parameterType = AnimationParameterType.Bool;
    
    public enum AnimationParameterType
    {
        Bool,
        Float,
        Trigger
    }
    
    [Header("Rotation")]
    [Tooltip("Rotation speed towards next waypoint")]
    public float rotationSpeed = 10f;
    [Tooltip("If true, character will rotate towards waypoint")]
    public bool rotateTowardsWaypoint = true;
    
    [Header("Debug")]
    [Tooltip("Show messages in console")]
    public bool showDebugLogs = true;
    [Tooltip("Draw lines between waypoints in Scene view")]
    public bool drawGizmos = true;
    public Color gizmoColor = Color.yellow;
    
    private int currentWaypointIndex = 0;
    private bool isRunning = false;
    private bool hasCompletedLoop = false;
    
    private Renderer[] renderers;
    private Collider[] colliders;
    private bool isVisible = false;
    
    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);
        
        ForceHide();
    }
    
    void Start()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("[WaypointRunner] No waypoints assigned!");
            enabled = false;
            return;
        }
        
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator != null && showDebugLogs)
            {
                Debug.Log("[WaypointRunner] Animator found automatically on this GameObject");
            }
        }
        
        if (animator != null && showDebugLogs)
        {
            Debug.Log($"[WaypointRunner] Animator controller: {animator.runtimeAnimatorController?.name}");
            Debug.Log($"[WaypointRunner] Animation parameter: {runAnimationParameter} (Type: {parameterType})");
        }
        
        if (startOnAwake)
        {
            Show();
            StartRunning();
        }
    }
    
    void Update()
    {
        if (!isRunning || waypoints == null || waypoints.Length == 0)
            return;
        
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        
        if (targetWaypoint == null)
        {
            Debug.LogWarning($"[WaypointRunner] Waypoint {currentWaypointIndex} is null!");
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            return;
        }
        
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        
        transform.position += direction * runSpeed * Time.deltaTime;
        
        if (rotateTowardsWaypoint && direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        float distanceToWaypoint = Vector3.Distance(transform.position, targetWaypoint.position);
        
        if (distanceToWaypoint <= waypointReachDistance)
        {
            if (showDebugLogs)
                Debug.Log($"[WaypointRunner] Reached waypoint {currentWaypointIndex}: {targetWaypoint.name}");
            
            if (currentWaypointIndex == 0 && hasCompletedLoop && disappearAfterComplete)
            {
                if (showDebugLogs)
                    Debug.Log("[WaypointRunner] Completed full loop and returned to start. Character disappearing...");
                StopRunning();
                gameObject.SetActive(false);
                return;
            }
            
            currentWaypointIndex++;
            
            if (currentWaypointIndex >= waypoints.Length)
            {
                if (loopWaypoints)
                {
                    currentWaypointIndex = 0;
                    hasCompletedLoop = true;
                    if (showDebugLogs)
                        Debug.Log("[WaypointRunner] Looping back to first waypoint");
                }
                else
                {
                    StopRunning();
                    if (showDebugLogs)
                        Debug.Log("[WaypointRunner] Finished all waypoints");
                    
                    if (disappearAfterComplete)
                    {
                        if (showDebugLogs)
                            Debug.Log("[WaypointRunner] Character disappearing...");
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    
    public void StartRunning()
    {
        Show();
        isRunning = true;
        
        if (animator != null && !string.IsNullOrEmpty(runAnimationParameter))
        {
            switch (parameterType)
            {
                case AnimationParameterType.Bool:
                    animator.SetBool(runAnimationParameter, true);
                    if (showDebugLogs) Debug.Log($"[WaypointRunner] Set animator bool '{runAnimationParameter}' to TRUE");
                    break;
                    
                case AnimationParameterType.Float:
                    animator.SetFloat(runAnimationParameter, runSpeed);
                    if (showDebugLogs) Debug.Log($"[WaypointRunner] Set animator float '{runAnimationParameter}' to {runSpeed}");
                    break;
                    
                case AnimationParameterType.Trigger:
                    animator.SetTrigger(runAnimationParameter);
                    if (showDebugLogs) Debug.Log($"[WaypointRunner] Triggered animator '{runAnimationParameter}'");
                    break;
            }
        }
        else if (showDebugLogs && animator == null)
        {
            Debug.LogWarning("[WaypointRunner] No Animator assigned - animation won't play!");
        }
        
        if (showDebugLogs)
            Debug.Log("[WaypointRunner] Started running");
    }
    
    public void StopRunning()
    {
        isRunning = false;
        
        if (animator != null && !string.IsNullOrEmpty(runAnimationParameter))
        {
            switch (parameterType)
            {
                case AnimationParameterType.Bool:
                    animator.SetBool(runAnimationParameter, false);
                    if (showDebugLogs) Debug.Log($"[WaypointRunner] Set animator bool '{runAnimationParameter}' to FALSE");
                    break;
                    
                case AnimationParameterType.Float:
                    animator.SetFloat(runAnimationParameter, 0f);
                    if (showDebugLogs) Debug.Log($"[WaypointRunner] Set animator float '{runAnimationParameter}' to 0");
                    break;
            }
        }
        
        if (showDebugLogs)
            Debug.Log("[WaypointRunner] Stopped running");
    }
    
    public void ResetToFirstWaypoint()
    {
        currentWaypointIndex = 0;
        hasCompletedLoop = false;
        
        if (waypoints != null && waypoints.Length > 0 && waypoints[0] != null)
        {
            transform.position = waypoints[0].position;
        }
    }
    
    public void ResetAnomaly()
    {
        StopRunning();
        ForceHide();
        currentWaypointIndex = 0;
        hasCompletedLoop = false;
        if (waypoints != null && waypoints.Length > 0 && waypoints[0] != null)
        {
            transform.position = waypoints[0].position;
        }
    }
    
    private void Show()
    {
        if (isVisible) return;
        
        foreach (Renderer r in renderers)
            if (r != null) r.enabled = true;
        
        foreach (Collider c in colliders)
            if (c != null) c.enabled = true;
        
        isVisible = true;
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
    
    private void OnDrawGizmos()
    {
        if (!drawGizmos || waypoints == null || waypoints.Length == 0)
            return;
        
        Gizmos.color = gizmoColor;
        
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                Gizmos.DrawWireSphere(waypoints[i].position, 0.3f);
            }
        }
        
        if (waypoints[waypoints.Length - 1] != null)
        {
            Gizmos.DrawWireSphere(waypoints[waypoints.Length - 1].position, 0.3f);
        }
        
        if (loopWaypoints && waypoints.Length > 1 && waypoints[0] != null && waypoints[waypoints.Length - 1] != null)
        {
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.5f);
            Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
        }
    }
}
