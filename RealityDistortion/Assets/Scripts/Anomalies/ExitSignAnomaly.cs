using UnityEngine;

public class ExitSignAnomaly : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private GameObject signObject;
    [SerializeField] private GameObject bodyObject;
    
    [Header("Texture Settings")]
    [SerializeField] private Texture2D anomalyTexture;
    
    [Header("Emission Settings")]
    [SerializeField] private Color normalEmissionColor = Color.white;
    [SerializeField] private Color anomalyEmissionColor = Color.red;
    
    [Header("Flicker Settings")]
    [SerializeField] private int flickerCount = 5;
    [SerializeField] private float flickerSpeed = 0.15f;
    [SerializeField] private int flickerSessions = 5;
    
    [Header("Timing Settings")]
    [SerializeField] private float timeBeforeAnomaly = 20f;
    [SerializeField] private float anomalyDuration = 20f;
    
    private Renderer signRenderer;
    private Renderer bodyRenderer;
    private Texture normalTexture;
    private float timer = 0f;
    private float anomalyTimer = 0f;
    private bool anomalyActive = false;
    private bool anomalyShown = false;
    private bool isFlickering = false;
    private int currentFlicker = 0;
    private int currentSession = 0;
    private float flickerTimer = 0f;
    private float nextFlickerTime = 0f;
    private bool showingAnomalyTexture = false;

    void Start()
    {
        if (signObject != null)
        {
            signRenderer = signObject.GetComponent<Renderer>();
            if (signRenderer != null)
            {
                normalTexture = signRenderer.material.mainTexture;
            }
        }
        
        if (bodyObject != null)
        {
            bodyRenderer = bodyObject.GetComponent<Renderer>();
        }
        
        if (signRenderer == null && bodyRenderer == null)
        {
            enabled = false;
        }
        
        ResetAnomaly();
    }
    
    private void OnEnable()
    {
        ResetAnomaly();
    }
    
    public void ResetAnomaly()
    {
        timer = 0f;
        anomalyTimer = 0f;
        anomalyActive = false;
        anomalyShown = false;
        isFlickering = false;
        currentFlicker = 0;
        currentSession = 0;
        flickerTimer = 0f;
        nextFlickerTime = 0f;
        showingAnomalyTexture = false;
        
        if (normalTexture != null && signRenderer != null)
        {
            signRenderer.material.mainTexture = normalTexture;
        }
        
        if (bodyRenderer != null)
        {
            bodyRenderer.material.SetColor("_EmissionColor", normalEmissionColor);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        
        if (!anomalyActive && !anomalyShown && timer >= timeBeforeAnomaly)
        {
            ActivateAnomaly();
        }
        
        if (anomalyActive)
        {
            anomalyTimer += Time.deltaTime;
            
            if (!isFlickering && currentSession < flickerSessions && anomalyTimer >= nextFlickerTime)
            {
                StartFlickerSession();
            }
            
            if (isFlickering)
            {
                flickerTimer += Time.deltaTime;
                
                if (flickerTimer >= flickerSpeed)
                {
                    flickerTimer = 0f;
                    showingAnomalyTexture = !showingAnomalyTexture;
                    
                    if (signRenderer != null)
                    {
                        signRenderer.material.mainTexture = showingAnomalyTexture ? anomalyTexture : normalTexture;
                    }
                    
                    currentFlicker++;
                    
                    if (currentFlicker >= flickerCount * 2)
                    {
                        EndFlickerSession();
                    }
                }
            }
        }
        
        if (anomalyActive && timer >= timeBeforeAnomaly + anomalyDuration)
        {
            DeactivateAnomaly();
        }
    }
    
    private void ActivateAnomaly()
    {
        anomalyActive = true;
        anomalyShown = true;
        anomalyTimer = 0f;
        currentSession = 0;
        
        float intervalBetweenSessions = anomalyDuration / flickerSessions;
        nextFlickerTime = 0f;
        
        if (bodyRenderer != null)
        {
            bodyRenderer.material.SetColor("_EmissionColor", anomalyEmissionColor);
        }
    }
    
    private void StartFlickerSession()
    {
        isFlickering = true;
        currentFlicker = 0;
        flickerTimer = 0f;
        showingAnomalyTexture = false;
        currentSession++;
    }
    
    private void EndFlickerSession()
    {
        isFlickering = false;
        
        if (signRenderer != null)
        {
            signRenderer.material.mainTexture = anomalyTexture;
        }
        
        if (currentSession < flickerSessions)
        {
            float intervalBetweenSessions = anomalyDuration / flickerSessions;
            nextFlickerTime = currentSession * intervalBetweenSessions;
        }
    }
    
    private void DeactivateAnomaly()
    {
        anomalyActive = false;
        
        if (normalTexture != null && signRenderer != null)
        {
            signRenderer.material.mainTexture = normalTexture;
        }
        
        if (bodyRenderer != null)
        {
            bodyRenderer.material.SetColor("_EmissionColor", normalEmissionColor);
        }
    }
}
