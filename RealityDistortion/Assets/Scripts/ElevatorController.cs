using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [Header("Elevator Settings")]
    [SerializeField] private string elevatorName = "Elevator";
    [SerializeField] private bool isGoodElevator = true;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    
    public void OnButtonPressed()
    {
        if (showDebugLogs)
        {
            string elevatorType = isGoodElevator ? "Good" : "Bad";
            Debug.Log($"[{elevatorName}] Button pressed! Elevator type: {elevatorType}");
        }
        
        CallElevator();
    }
    
    private void CallElevator()
    {
        if (showDebugLogs)
        {
            Debug.Log($"[{elevatorName}] Calling elevator...");
        }
        
    }
}
