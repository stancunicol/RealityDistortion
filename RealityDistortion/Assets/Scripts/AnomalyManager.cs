using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class AnomalyManager : MonoBehaviour
{
    [Header("Level System")]
    [Tooltip("Current floor level (0-3 for 4 floors)")]
    [SerializeField] private int currentLevel = 0;
    [Tooltip("Maximum number of anomalies per floor")]
    [Range(0, 3)]
    [SerializeField] private int maxAnomaliesPerFloor = 3;
    
    [Header("Config")]
    [Tooltip("If true, will automatically find all anomalies in the scene")]
    [SerializeField] private bool autoFindAnomalies = true;
    
    [Header("Elevator Display")]
    [SerializeField] private TextMeshPro[] floorDisplayTexts;
    [SerializeField] private string floorPrefix = "FLOOR ";
    
    [Header("Game Over")]
    public UnityEvent onGameOver;
    [SerializeField] private string gameOverMessage = "GAME OVER\nYou made the wrong choice...";
    [SerializeField] private int gameOverFontSize = 48;
    [SerializeField] private Color gameOverTextColor = Color.red;
    [SerializeField] private float gameOverDelay = 3f;
    [SerializeField] private float gameOverFadeDuration = 1f;
    [SerializeField] private string mainMenuSceneName = "MenuScene";
    
    [Header("Victory")]
    public UnityEvent onVictory;
    [SerializeField] private string victoryMessage = "YOU ESCAPED!\nCongratulations!";
    [SerializeField] private int victoryFontSize = 48;
    [SerializeField] private Color victoryTextColor = Color.green;
    [SerializeField] private float victoryDelay = 5f;
    [SerializeField] private float victoryFadeDuration = 1f;
    
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
        DisableAllAnomalyScripts();
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
        FindAnomaliesByType<AnomalyLamp>();
        FindAnomaliesByType<WomanSculpture>();
        FindAnomaliesByType<WaypointRunner>();
        
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
        
        var anomalyGroups = new Dictionary<System.Type, List<GameObject>>();
        
        foreach (GameObject anomaly in allAnomaliesInScene)
        {
            if (anomaly == null) continue;
            
            System.Type anomalyType = null;
            if (anomaly.GetComponent<AnomalyLamp>() != null) anomalyType = typeof(AnomalyLamp);
            else if (anomaly.GetComponent<BedroomLampController>() != null) anomalyType = typeof(BedroomLampController);
            else if (anomaly.GetComponent<ClockPainting>() != null) anomalyType = typeof(ClockPainting);
            else if (anomaly.GetComponent<CreepyPainting>() != null) anomalyType = typeof(CreepyPainting);
            else if (anomaly.GetComponent<DoorClose>() != null) anomalyType = typeof(DoorClose);
            else if (anomaly.GetComponent<WomanSculpture>() != null) anomalyType = typeof(WomanSculpture);
            else if (anomaly.GetComponent<TV>() != null) anomalyType = typeof(TV);
            else if (anomaly.GetComponent<Doll>() != null) anomalyType = typeof(Doll);
            else if (anomaly.GetComponent<ExitSignAnomaly>() != null) anomalyType = typeof(ExitSignAnomaly);
            else if (anomaly.GetComponent<SuitcaseAnomaly>() != null) anomalyType = typeof(SuitcaseAnomaly);
            else if (anomaly.GetComponent<FootstepZoneDirect>() != null) anomalyType = typeof(FootstepZoneDirect);
            else if (anomaly.GetComponent<WaypointRunner>() != null) anomalyType = typeof(WaypointRunner);
            else if (anomaly.GetComponent<AppearingCharacterAnomaly>() != null) anomalyType = typeof(AppearingCharacterAnomaly);
            
            if (anomalyType != null)
            {
                if (!anomalyGroups.ContainsKey(anomalyType))
                {
                    anomalyGroups[anomalyType] = new List<GameObject>();
                }
                anomalyGroups[anomalyType].Add(anomaly);
            }
        }
        
        var uniqueAnomalies = new List<GameObject>();
        foreach (var group in anomalyGroups.Values)
        {
            if (group.Count > 0)
            {
                uniqueAnomalies.Add(group[Random.Range(0, group.Count)]);
            }
        }
        
        for (int level = 0; level < 4; level++)
        {
            int anomalyCountForLevel;
            
            float randomChance = Random.Range(0f, 1f);
            if (randomChance < 0.1f)
            {
                anomalyCountForLevel = 0;
            }
            else
            {
                anomalyCountForLevel = Random.Range(1, maxAnomaliesPerFloor + 1);
            }
            
            List<GameObject> shuffled = uniqueAnomalies.OrderBy(x => Random.value).ToList();
            List<GameObject> selectedAnomalies = shuffled.Take(anomalyCountForLevel).ToList();
            
            levelAnomalies[level] = selectedAnomalies;
            
            if (showDebugLogs)
            {
                Debug.Log($"[AnomalyManager] Level {level}: Generated {anomalyCountForLevel} anomalies");
                foreach (GameObject anomaly in selectedAnomalies)
                {
                    string anomalyTypeName = GetAnomalyTypeName(anomaly);
                    Debug.Log($"  - {anomaly.name} ({anomalyTypeName})");
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
        
        ResetAllAnomalies();
        
        DisableAllAnomalyScripts();
        
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
                EnableAnomalyScript(anomaly);
                if (showDebugLogs)
                {
                    string anomalyTypeName = GetAnomalyTypeName(anomaly);
                    Debug.Log($"[AnomalyManager] Activated anomaly script: {anomalyTypeName}");
                }
            }
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"[AnomalyManager] Level {level} loaded with {activeAnomalies.Count} anomalies");
        }
        
        UpdateFloorDisplay();
    }
    
    public void NextLevel()
    {
        StartCoroutine(NextLevelCoroutine());
    }
    
    private IEnumerator NextLevelCoroutine()
    {
        yield return new WaitForSeconds(3f);
        
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
    
    private void UpdateFloorDisplay()
    {
        if (floorDisplayTexts != null && floorDisplayTexts.Length > 0)
        {
            int displayFloor = 3 - currentLevel;
            foreach (TextMeshPro displayText in floorDisplayTexts)
            {
                if (displayText != null)
                {
                    displayText.text = displayFloor.ToString();
                }
            }
        }
    }
    
    private void ResetAllAnomalies()
    {
        foreach (GameObject anomaly in allAnomaliesInScene)
        {
            if (anomaly != null)
            {
                ResetAnomalyToInitialState(anomaly);
            }
        }
    }
    
    private void DisableAllAnomalyScripts()
    {
        foreach (GameObject anomaly in allAnomaliesInScene)
        {
            if (anomaly != null)
            {
                DisableAnomalyScript(anomaly);
            }
        }
        
        activeAnomalies.Clear();
    }
    
    private void ResetAnomalyToInitialState(GameObject anomaly)
    {
        var lamp = anomaly.GetComponent<AnomalyLamp>();
        if (lamp != null)
        {
            lamp.ResetAnomaly();
        }
        
        var bedroomLamp = anomaly.GetComponent<BedroomLampController>();
        if (bedroomLamp != null)
        {
            bedroomLamp.ResetAnomaly();
        }
        
        var clockPainting = anomaly.GetComponent<ClockPainting>();
        if (clockPainting != null)
        {
            clockPainting.ResetAnomaly();
        }
        
        var creepyPainting = anomaly.GetComponent<CreepyPainting>();
        if (creepyPainting != null)
        {
            creepyPainting.ResetAnomaly();
        }
        
        var doorClose = anomaly.GetComponent<DoorClose>();
        if (doorClose != null)
        {
            doorClose.ResetAnomaly();
        }
        
        var womanSculpture = anomaly.GetComponent<WomanSculpture>();
        if (womanSculpture != null)
        {
        }
        
        var tv = anomaly.GetComponent<TV>();
        if (tv != null)
        {
            tv.ResetAnomaly();
        }
        
        var doll = anomaly.GetComponent<Doll>();
        if (doll != null)
        {
            doll.ResetAnomaly();
        }
        
        var exitSign = anomaly.GetComponent<ExitSignAnomaly>();
        if (exitSign != null)
        {
            exitSign.ResetAnomaly();
        }
        
        var suitcase = anomaly.GetComponent<SuitcaseAnomaly>();
        if (suitcase != null)
        {
            suitcase.ResetAnomaly();
        }
        
        var footstep = anomaly.GetComponent<FootstepZoneDirect>();
        if (footstep != null)
        {
            footstep.ResetAnomaly();
        }
        
        var waypointRunner = anomaly.GetComponent<WaypointRunner>();
        if (waypointRunner != null)
        {
            waypointRunner.ResetAnomaly();
        }
        
        var appearingChar = anomaly.GetComponent<AppearingCharacterAnomaly>();
        if (appearingChar != null)
        {
            appearingChar.ResetAnomaly();
        }
    }
    
    private void DisableAnomalyScript(GameObject anomaly)
    {
        var lamp = anomaly.GetComponent<AnomalyLamp>();
        if (lamp != null) lamp.enabled = false;
        
        var bedroomLamp = anomaly.GetComponent<BedroomLampController>();
        if (bedroomLamp != null) bedroomLamp.enabled = false;
        
        var clockPainting = anomaly.GetComponent<ClockPainting>();
        if (clockPainting != null) clockPainting.enabled = false;
        
        var creepyPainting = anomaly.GetComponent<CreepyPainting>();
        if (creepyPainting != null) creepyPainting.enabled = false;
        
        var doorClose = anomaly.GetComponent<DoorClose>();
        if (doorClose != null) doorClose.enabled = false;
        
        var womanSculpture = anomaly.GetComponent<WomanSculpture>();
        if (womanSculpture != null) womanSculpture.enabled = false;
        
        var tv = anomaly.GetComponent<TV>();
        if (tv != null) tv.enabled = false;
        
        var doll = anomaly.GetComponent<Doll>();
        if (doll != null) doll.enabled = false;
        
        var exitSign = anomaly.GetComponent<ExitSignAnomaly>();
        if (exitSign != null) exitSign.enabled = false;
        
        var suitcase = anomaly.GetComponent<SuitcaseAnomaly>();
        if (suitcase != null) suitcase.enabled = false;
        
        var footstep = anomaly.GetComponent<FootstepZoneDirect>();
        if (footstep != null) footstep.enabled = false;
        
        var waypointRunner = anomaly.GetComponent<WaypointRunner>();
        if (waypointRunner != null) waypointRunner.enabled = false;
        
        var appearingChar = anomaly.GetComponent<AppearingCharacterAnomaly>();
        if (appearingChar != null) appearingChar.enabled = false;
    }
    
    private void EnableAnomalyScript(GameObject anomaly)
    {
        var lamp = anomaly.GetComponent<AnomalyLamp>();
        if (lamp != null) { lamp.enabled = true; return; }
        
        var bedroomLamp = anomaly.GetComponent<BedroomLampController>();
        if (bedroomLamp != null) { bedroomLamp.enabled = true; return; }
        
        var clockPainting = anomaly.GetComponent<ClockPainting>();
        if (clockPainting != null) { clockPainting.enabled = true; return; }
        
        var creepyPainting = anomaly.GetComponent<CreepyPainting>();
        if (creepyPainting != null) { creepyPainting.enabled = true; return; }
        
        var doorClose = anomaly.GetComponent<DoorClose>();
        if (doorClose != null) { doorClose.enabled = true; return; }
        
        var womanSculpture = anomaly.GetComponent<WomanSculpture>();
        if (womanSculpture != null) { womanSculpture.enabled = true; return; }
        
        var tv = anomaly.GetComponent<TV>();
        if (tv != null) { tv.enabled = true; return; }
        
        var doll = anomaly.GetComponent<Doll>();
        if (doll != null) { doll.enabled = true; return; }
        
        var exitSign = anomaly.GetComponent<ExitSignAnomaly>();
        if (exitSign != null) { exitSign.enabled = true; return; }
        
        var suitcase = anomaly.GetComponent<SuitcaseAnomaly>();
        if (suitcase != null) { suitcase.enabled = true; return; }
        
        var footstep = anomaly.GetComponent<FootstepZoneDirect>();
        if (footstep != null) { footstep.enabled = true; return; }
        
        var waypointRunner = anomaly.GetComponent<WaypointRunner>();
        if (waypointRunner != null) 
        { 
            waypointRunner.enabled = true;
            waypointRunner.StartRunning();
            return; 
        }
        
        var appearingChar = anomaly.GetComponent<AppearingCharacterAnomaly>();
        if (appearingChar != null) { appearingChar.enabled = true; return; }
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
            float fadeAlpha = Mathf.Clamp01(gameOverTimer / gameOverFadeDuration);
            
            GUI.color = new Color(0, 0, 0, fadeAlpha);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = gameOverFontSize;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = new Color(gameOverTextColor.r, gameOverTextColor.g, gameOverTextColor.b, fadeAlpha);
            style.alignment = TextAnchor.MiddleCenter;
            
            float width = Screen.width;
            float height = 200;
            float x = 0;
            float y = (Screen.height - height) / 2;
            
            GUI.color = new Color(0, 0, 0, 0.8f * fadeAlpha);
            GUI.Label(new Rect(x + 3, y + 3, width, height), gameOverMessage, style);
            
            GUI.color = new Color(1, 1, 1, fadeAlpha);
            GUI.Label(new Rect(x, y, width, height), gameOverMessage, style);
        }
        
        if (hasWon)
        {
            float fadeAlpha = Mathf.Clamp01(victoryTimer / victoryFadeDuration);
            
            GUI.color = new Color(0, 0, 0, fadeAlpha);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
            
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = victoryFontSize;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = new Color(victoryTextColor.r, victoryTextColor.g, victoryTextColor.b, fadeAlpha);
            style.alignment = TextAnchor.MiddleCenter;
            
            float width = Screen.width;
            float height = 200;
            float x = 0;
            float y = (Screen.height - height) / 2;
            
            GUI.color = new Color(0, 0, 0, 0.8f * fadeAlpha);
            GUI.Label(new Rect(x + 3, y + 3, width, height), victoryMessage, style);
            
            GUI.color = new Color(1, 1, 1, fadeAlpha);
            GUI.Label(new Rect(x, y, width, height), victoryMessage, style);
        }
    }
    
    private string GetAnomalyTypeName(GameObject anomaly)
    {
        if (anomaly == null) return "Unknown";
        
        if (anomaly.GetComponent<AnomalyLamp>() != null) return "Flickering Lamp";
        if (anomaly.GetComponent<BedroomLampController>() != null) return "Color Changing Lamp";
        if (anomaly.GetComponent<ClockPainting>() != null) return "Clock Painting";
        if (anomaly.GetComponent<CreepyPainting>() != null) return "Creepy Painting";
        if (anomaly.GetComponent<DoorClose>() != null) return "Door Close";
        if (anomaly.GetComponent<WomanSculpture>() != null) return "Woman Sculpture";
        if (anomaly.GetComponent<TV>() != null) return "TV";
        if (anomaly.GetComponent<Doll>() != null) return "Doll";
        if (anomaly.GetComponent<ExitSignAnomaly>() != null) return "Exit Sign";
        if (anomaly.GetComponent<SuitcaseAnomaly>() != null) return "Suitcase";
        if (anomaly.GetComponent<FootstepZoneDirect>() != null) return "Footsteps";
        if (anomaly.GetComponent<WaypointRunner>() != null) return "Running Character";
        if (anomaly.GetComponent<AppearingCharacterAnomaly>() != null) return "Bad Guy";
        
        return anomaly.name;
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
