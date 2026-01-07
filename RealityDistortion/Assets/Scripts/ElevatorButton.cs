using UnityEngine;
using UnityEngine.Events;

public class ElevatorButton : MonoBehaviour
{
    [Header("Button Type")]
    [SerializeField] private bool isGoodButton = true;
    [SerializeField] private string buttonName = "Elevator Button";
    
    [Header("Button Settings")]
    [Tooltip("Maxim distance from which the button can be pressed")]
    [SerializeField] private float maxInteractionDistance = 5f;
    [Tooltip("Angle tolerance for button detection (higher = easier to look at)")]
    [SerializeField] private float detectionAngle = 15f;
    
    [Header("Visual Feedback")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material pressedMaterial;
    [SerializeField] private float pressedDuration = 0.5f;
    
    [Header("Hover Message")]
    [SerializeField] private string hoverMessage = "Press E or Click to call elevator";
    [SerializeField] private int messageFontSize = 24;
    [SerializeField] private Color messageColor = Color.white;
    
    [Header("Audio")]
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioSource audioSource;
    
    [Header("Events")]
    [Tooltip("Event called when button is pressed")]
    public UnityEvent onButtonPressed;
    
    private MeshRenderer meshRenderer;
    private bool isPressed = false;
    private float pressedTimer = 0f;
    private Transform playerCamera;
    private bool isHovering = false;
    
    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        
        if (meshRenderer != null && normalMaterial != null)
        {
            meshRenderer.material = normalMaterial;
        }
        
        if (Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }
        
        // Configurare AudioSource
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D sound
        }
        
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }
    
    private void Update()
    {
        if (!enabled)
            return;
            
        CheckHover();
        
        if (isPressed)
        {
            pressedTimer += Time.deltaTime;
            if (pressedTimer >= pressedDuration)
            {
                ResetButton();
            }
        }
        
        if (Input.GetMouseButtonDown(0) && isHovering)
        {
            PressButton();
        }
        
        if (Input.GetKeyDown(KeyCode.E) && isHovering)
        {
            PressButton();
        }
    }
    
    private void OnDisable()
    {
        isHovering = false;
    }
    
    private void CheckHover()
    {
        if (playerCamera == null)
            return;
        
        Vector3 directionToButton = (transform.position - playerCamera.position).normalized;
        float angleToButton = Vector3.Angle(playerCamera.forward, directionToButton);
        float distanceToButton = Vector3.Distance(playerCamera.position, transform.position);
        
        bool isLookingAtButton = angleToButton <= detectionAngle && distanceToButton <= maxInteractionDistance;
        
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        bool hitButton = false;
        
        if (Physics.Raycast(ray, out RaycastHit hit, maxInteractionDistance))
        {
            if (hit.transform == transform)
            {
                hitButton = true;
            }
        }
        
        if (isLookingAtButton || hitButton)
        {
            if (!isHovering)
            {
                isHovering = true;
                ShowHoverMessage();
            }
            return;
        }
        
        if (isHovering)
        {
            isHovering = false;
            HideHoverMessage();
        }
    }
    
    private void ShowHoverMessage()
    {
    }
    
    private void HideHoverMessage()
    {
    }
    
    private void OnGUI()
    {
        if (!isHovering)
            return;
        
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = messageFontSize;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = messageColor;
        style.alignment = TextAnchor.MiddleCenter;
        
        float width = 500;
        float height = 60;
        float x = (Screen.width - width) / 2;
        float y = Screen.height - 150;
        
        GUI.color = new Color(0, 0, 0, 0.8f);
        GUI.Label(new Rect(x + 2, y + 2, width, height), hoverMessage, style);
        
        GUI.color = Color.white;
        GUI.Label(new Rect(x, y, width, height), hoverMessage, style);
    }
    
    private void PressButton()
    {
        isPressed = true;
        pressedTimer = 0f;
        
        if (meshRenderer != null && pressedMaterial != null)
        {
            meshRenderer.material = pressedMaterial;
        }
        
        // Play button sound
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
        
        // Activează secvența ușilor de lift
        ElevatorDoor elevatorDoor = FindObjectOfType<ElevatorDoor>();
        if (elevatorDoor != null)
        {
            elevatorDoor.ActivateElevator();
        }
        
        CheckAnswerAndProceed();
        
        onButtonPressed?.Invoke();
    }
    
    private void CheckAnswerAndProceed()
    {
        AnomalyManager anomalyManager = FindObjectOfType<AnomalyManager>();
        
        if (anomalyManager == null)
        {
            Debug.LogWarning("[ElevatorButton] AnomalyManager not found in scene!");
            return;
        }
        
        bool playerSaysAnomaliesExist = isGoodButton;
        bool isCorrect = anomalyManager.CheckPlayerChoice(playerSaysAnomaliesExist);
        
        if (isCorrect)
        {
            Debug.Log($"[ElevatorButton] Correct choice! Moving to next level...");
            anomalyManager.NextLevel();
        }
        else
        {
            Debug.Log($"[ElevatorButton] Wrong choice! GAME OVER!");
            anomalyManager.TriggerGameOver();
        }
    }
    
    private void ResetButton()
    {
        isPressed = false;
        
        if (meshRenderer != null && normalMaterial != null)
        {
            meshRenderer.material = normalMaterial;
        }
    }
}
