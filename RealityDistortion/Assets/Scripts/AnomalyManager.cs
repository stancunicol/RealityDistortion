using UnityEngine;
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
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    
    private List<GameObject> activeAnomalies = new List<GameObject>();
    private List<GameObject> allAnomaliesInScene = new List<GameObject>();
    private Dictionary<int, List<GameObject>> levelAnomalies = new Dictionary<int, List<GameObject>>();
    
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
                Debug.Log("[AnomalyManager] Already at max level (3)");
        }
    }
    
    public void PreviousLevel()
    {
        int prevLevel = currentLevel - 1;
        if (prevLevel >= 0)
        {
            LoadLevelAnomalies(prevLevel);
        }
        else
        {
            if (showDebugLogs)
                Debug.Log("[AnomalyManager] Already at min level (0)");
        }
    }
    
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    
    public void SelectAndActivateAnomalies()
    {
        LoadLevelAnomalies(currentLevel);
    }
    
    private void DeactivateAllAnomalies()
    {
        foreach (GameObject anomaly in allAnomaliesInScene)
        {
            if (anomaly != null)
            {
                anomaly.SetActive(false);
            }
        }
        
        activeAnomalies.Clear();
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
    
    public List<GameObject> GetActiveAnomalies()
    {
        return new List<GameObject>(activeAnomalies);
    }
    
    public bool IsAnomalyActive(GameObject anomaly)
    {
        return activeAnomalies.Contains(anomaly);
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

#if UNITY_EDITOR
    [ContextMenu("Refresh Anomalies List")]
    private void RefreshAnomaliesList()
    {
        FindAllAnomaliesInScene();
        GenerateAnomaliesForAllLevels();
    }
    
    [ContextMenu("Test Select Anomalies")]
    private void TestSelectAnomalies()
    {
        SelectAndActivateAnomalies();
    }
    
    [ContextMenu("Go to Next Level")]
    private void TestNextLevel()
    {
        NextLevel();
    }
    
    [ContextMenu("Go to Previous Level")]
    private void TestPreviousLevel()
    {
        PreviousLevel();
    }
#endif
}
