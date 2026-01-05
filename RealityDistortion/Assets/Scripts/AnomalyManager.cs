using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class AnomalyManager : MonoBehaviour
{
    [Header("Level System")]
    [Tooltip("Current floor level (0-3 for 4 floors)")]
    [SerializeField] private int currentLevel = 0;
    [Tooltip("Maximum number of anomalies per floor")]
    [Range(0, 2)]
    [SerializeField] private int maxAnomaliesPerFloor = 2;
    
    [Header("Config")]
    [Tooltip("If true, will automatically find all anomalies in the scene")]
    [SerializeField] private bool autoFindAnomalies = true;
    
    [Header("Game Over")]
    public UnityEvent onGameOver;
    [SerializeField] private string gameOverMessage = "GAME OVER\nYou made the wrong choice...";
    [SerializeField] private int gameOverFontSize = 48;
    [SerializeField] private Color gameOverTextColor = Color.red;
    [SerializeField] private float gameOverDelay = 3f;
    [SerializeField] private string mainMenuSceneName = "MenuScene";
    
    [Header("Victory")]
    public UnityEvent onVictory;
    [SerializeField] private string victoryMessage = "YOU ESCAPED!\nCongratulations!";
    [SerializeField] private int victoryFontSize = 48;
    [SerializeField] private Color victoryTextColor = Color.green;
    [SerializeField] private float victoryDelay = 5f;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    
    private List<GameObject> activeAnomalies = new List<GameObject>();
    private List<GameObject> allAnomaliesInScene = new List<GameObject>();
    private Dictionary<int, List<GameObject>> levelAnomalies = new Dictionary<int, List<GameObject>>();
    private bool isGameOver = false;
    private float gameOverTimer = 0f;
    private bool hasWon = false;
    private float victoryTimer = 0f;
    
    private void Awake()
    {
        if (autoFindAnomalies)
        {
            FindAllAnomaliesInScene();
        }
        
        GenerateAnomaliesForAllLevels();
    }
    
    private void Start()
    {
        LoadLevelAnomalies(currentLevel);
    }
    
    private void Update()
    {
        if (isGameOver)
        {
            gameOverTimer += Time.unscaledDeltaTime;
            
            if (gameOverTimer >= gameOverDelay)
            {
                LoadMainMenu();
            }
        }
        
        if (hasWon)
        {
            victoryTimer += Time.unscaledDeltaTime;
            
            if (victoryTimer >= victoryDelay)
            {
                LoadMainMenu();
            }
        }
    }
    
    private void FindAllAnomaliesInScene()
    {
        allAnomaliesInScene.Clear();
        
        FindAnomaliesByType<AppearingCharacterAnomaly>();
        FindAnomaliesByType<TV>();
        FindAnomaliesByType<Doll>();
        FindAnomaliesByType<ExitSignAnomaly>();
        FindAnomaliesByType<SuitcaseAnomaly>();
        FindAnomaliesByType<CreepyPainting>();
        FindAnomaliesByType<DoorClose>();
        FindAnomaliesByType<FootstepZoneDirect>();
        FindAnomaliesByType<BedroomLampController>();
        FindAnomaliesByType<ClockPainting>();
        
        if (showDebugLogs)
            Debug.Log($"[AnomalyManager] Found {allAnomaliesInScene.Count} anomalies in the scene");
    }
    
    private void FindAnomaliesByType<T>() where T : MonoBehaviour
    {
        T[] anomalies = FindObjectsOfType<T>(true);
        foreach (T anomaly in anomalies)
        {
            if (!allAnomaliesInScene.Contains(anomaly.gameObject))
            {
                allAnomaliesInScene.Add(anomaly.gameObject);
                if (showDebugLogs)
                    Debug.Log($"[AnomalyManager] Found anomaly: {anomaly.gameObject.name} ({typeof(T).Name})");
            }
        }
    }
    
    private void GenerateAnomaliesForAllLevels()
    {
        levelAnomalies.Clear();
        
        for (int level = 0; level < 4; level++)
        {
            int anomalyCountForLevel = Random.Range(0, maxAnomaliesPerFloor + 1);
            
            List<GameObject> shuffled = allAnomaliesInScene.OrderBy(x => Random.value).ToList();
            List<GameObject> selectedAnomalies = shuffled.Take(anomalyCountForLevel).ToList();
            
            levelAnomalies[level] = selectedAnomalies;
            
            if (showDebugLogs)
            {
                Debug.Log($"[AnomalyManager] Level {level}: Generated {anomalyCountForLevel} anomalies");
                foreach (GameObject anomaly in selectedAnomalies)
                {
                    Debug.Log($"  - {anomaly.name}");
                }
            }
        }
    }
    
    public void LoadLevelAnomalies(int level)
    {
        if (level < 0 || level > 3)
        {
            Debug.LogWarning($"[AnomalyManager] Invalid level {level}. Must be between 0-3");
            return;
        }
        
        currentLevel = level;
        
        ResetAndDeactivateAllAnomalies();
        
        if (!levelAnomalies.ContainsKey(level))
        {
            Debug.LogWarning($"[AnomalyManager] No anomalies generated for level {level}");
            return;
        }
        
        activeAnomalies = levelAnomalies[level];
        
        foreach (GameObject anomaly in activeAnomalies)
        {
            if (anomaly != null)
            {
                anomaly.SetActive(true);
                if (showDebugLogs)
                    Debug.Log($"[AnomalyManager] Activated anomaly: {anomaly.name}");
            }
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"[AnomalyManager] Level {level} loaded with {activeAnomalies.Count} anomalies");
        }
    }
    
    public void NextLevel()
    {
        int nextLevel = currentLevel + 1;
        if (nextLevel <= 3)
        {
            LoadLevelAnomalies(nextLevel);
        }
        else
        {
            if (showDebugLogs)
                Debug.Log("[AnomalyManager] Player completed all levels - Victory!");
            
            TriggerVictory();
        }
    }
    
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    
    private void ResetAndDeactivateAllAnomalies()
    {
        foreach (GameObject anomaly in allAnomaliesInScene)
        {
            if (anomaly != null)
            {
                anomaly.SetActive(false);
                anomaly.SetActive(true);
                
                Animator animator = anomaly.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.Rebind();
                    animator.Update(0f);
                }
                
                MonoBehaviour[] scripts = anomaly.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour script in scripts)
                {
                    if (script != null && script.enabled)
                    {
                        script.enabled = false;
                        script.enabled = true;
                    }
                }
                
                anomaly.SetActive(false);
                
                if (showDebugLogs)
                    Debug.Log($"[AnomalyManager] Reset anomaly: {anomaly.name}");
            }
        }
        
        activeAnomalies.Clear();
    }
    
    public bool CurrentLevelHasAnomalies()
    {
        if (!levelAnomalies.ContainsKey(currentLevel))
            return false;
        
        return levelAnomalies[currentLevel].Count > 0;
    }
    
    public int GetCurrentLevelAnomalyCount()
    {
        if (!levelAnomalies.ContainsKey(currentLevel))
            return 0;
        
        return levelAnomalies[currentLevel].Count;
    }
    
    public bool CheckPlayerChoice(bool playerSaysAnomaliesExist)
    {
        bool actuallyHasAnomalies = CurrentLevelHasAnomalies();
        bool isCorrect = playerSaysAnomaliesExist == actuallyHasAnomalies;
        
        if (showDebugLogs)
        {
            string playerChoice = playerSaysAnomaliesExist ? "ANOMALIES EXIST" : "NO ANOMALIES";
            string reality = actuallyHasAnomalies ? "has anomalies" : "has NO anomalies";
            string result = isCorrect ? "CORRECT" : "WRONG";
            
            Debug.Log($"[AnomalyManager] Level {currentLevel} {reality}");
            Debug.Log($"[AnomalyManager] Player chose: {playerChoice} - {result}!");
        }
        
        return isCorrect;
    }
    
    public void TriggerGameOver()
    {
        if (showDebugLogs)
            Debug.Log("[AnomalyManager] GAME OVER - Wrong choice!");
        
        isGameOver = true;
        gameOverTimer = 0f;
        
        // Disable all elevator buttons
        ElevatorButton[] buttons = FindObjectsOfType<ElevatorButton>();
        foreach (ElevatorButton button in buttons)
        {
            button.enabled = false;
        }
        
        onGameOver?.Invoke();
        
        Time.timeScale = 0f;
    }
    
    public void TriggerVictory()
    {
        if (showDebugLogs)
            Debug.Log("[AnomalyManager] VICTORY - You escaped!");
        
        hasWon = true;
        victoryTimer = 0f;
        
        // Disable all elevator buttons
        ElevatorButton[] buttons = FindObjectsOfType<ElevatorButton>();
        foreach (ElevatorButton button in buttons)
        {
            button.enabled = false;
        }
        
        onVictory?.Invoke();
        
        Time.timeScale = 0f;
    }
    
    private void LoadMainMenu()
    {
        Time.timeScale = 1f;
        
        // Unlock and show cursor for menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if (showDebugLogs)
            Debug.Log($"[AnomalyManager] Loading main menu: {mainMenuSceneName}");
        
        SceneManager.LoadScene(mainMenuSceneName);
    }
    
    private void OnGUI()
    {
        if (isGameOver)
        {
            // Draw black background
            GUI.color = Color.black;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            
            // Draw game over text
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = gameOverFontSize;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = gameOverTextColor;
            style.alignment = TextAnchor.MiddleCenter;
            
            float width = Screen.width;
            float height = 200;
            float x = 0;
            float y = (Screen.height - height) / 2;
            
            // Draw shadow
            GUI.color = new Color(0, 0, 0, 0.8f);
            GUI.Label(new Rect(x + 3, y + 3, width, height), gameOverMessage, style);
            
            // Draw main text
            GUI.color = Color.white;
            GUI.Label(new Rect(x, y, width, height), gameOverMessage, style);
        }
        
        if (hasWon)
        {
            // Draw black background
            GUI.color = Color.black;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            
            // Draw victory text
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = victoryFontSize;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = victoryTextColor;
            style.alignment = TextAnchor.MiddleCenter;
            
            float width = Screen.width;
            float height = 200;
            float x = 0;
            float y = (Screen.height - height) / 2;
            
            // Draw shadow
            GUI.color = new Color(0, 0, 0, 0.8f);
            GUI.Label(new Rect(x + 3, y + 3, width, height), victoryMessage, style);
            
            // Draw main text
            GUI.color = Color.white;
            GUI.Label(new Rect(x, y, width, height), victoryMessage, style);
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Refresh Anomalies List")]
    private void RefreshAnomaliesList()
    {
        FindAllAnomaliesInScene();
        GenerateAnomaliesForAllLevels();
    }
#endif
}
