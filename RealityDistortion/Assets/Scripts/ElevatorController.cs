using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [Header("Elevator Settings")]
    [SerializeField] private string elevatorName = "Elevator";
    [SerializeField] private bool isGoodElevator = true;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    
    /// <summary>
    /// Called when the elevator button is pressed
    /// </summary>
    public void OnButtonPressed()
    {
        if (showDebugLogs)
        {
            string elevatorType = isGoodElevator ? "Good" : "Bad";
            Debug.Log($"[{elevatorName}] Button pressed! Elevator type: {elevatorType}");
        }
        
        // TODO: Add elevator movement logic here
        CallElevator();
    }
    
    private void CallElevator()
    {
        if (showDebugLogs)
        {
            Debug.Log($"[{elevatorName}] Calling elevator...");
        }
        
        // Add your elevator logic here
        // For example: move elevator, open doors, etc.
    }
}
