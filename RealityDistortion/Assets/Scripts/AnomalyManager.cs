using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AnomalyManager : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("How many anomalies to activate in the scene")]
    [Range(0, 10)]
    [SerializeField] private int anomalyCount = 2;
    
    [Tooltip("If true, will automatically find all anomalies in the scene")]
    [SerializeField] private bool autoFindAnomalies = true;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    
    private List<GameObject> activeAnomalies = new List<GameObject>();
    private List<GameObject> allAnomaliesInScene = new List<GameObject>();
    
    private void Awake()
    {
        if (autoFindAnomalies)
        {
            FindAllAnomaliesInScene();
        }
    }
    
    private void Start()
    {
        SelectAndActivateAnomalies();
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
    
    public void SelectAndActivateAnomalies()
    {
        DeactivateAllAnomalies();
        
        if (allAnomaliesInScene.Count == 0)
        {
            Debug.LogWarning("[AnomalyManager] No anomalies found in the scene");
            return;
        }
        
        if (anomalyCount == 0)
        {
            if (showDebugLogs)
                Debug.Log("[AnomalyManager] Anomaly count set to 0 - no anomalies will be activated");
            return;
        }
        
        int anomaliesToSelect = Mathf.Min(anomalyCount, allAnomaliesInScene.Count);
        
        List<GameObject> shuffled = allAnomaliesInScene.OrderBy(x => Random.value).ToList();
        activeAnomalies = shuffled.Take(anomaliesToSelect).ToList();
        
        foreach (GameObject anomaly in activeAnomalies)
        {
            anomaly.SetActive(true);
                if (showDebugLogs)
                    Debug.Log($"[AnomalyManager] Activated anomaly: {anomaly.name}");
        }
        
        if (showDebugLogs)
        {
            Debug.Log($"[AnomalyManager] Activated {activeAnomalies.Count} out of {allAnomaliesInScene.Count} possible anomalies");
        }
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
    
    public List<GameObject> GetActiveAnomalies()
    {
        return new List<GameObject>(activeAnomalies);
    }
    
    public bool IsAnomalyActive(GameObject anomaly)
    {
        return activeAnomalies.Contains(anomaly);
    }

#if UNITY_EDITOR
    [ContextMenu("Refresh Anomalies List")]
    private void RefreshAnomaliesList()
    {
        FindAllAnomaliesInScene();
    }
    
    [ContextMenu("Test Select Anomalies")]
    private void TestSelectAnomalies()
    {
        SelectAndActivateAnomalies();
    }
#endif
}
