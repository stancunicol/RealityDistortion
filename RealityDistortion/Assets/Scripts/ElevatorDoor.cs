using UnityEngine;
using System.Collections;

public class ElevatorDoor : MonoBehaviour
{
    [Header("Door References - ASSIGN THESE IN INSPECTOR")]
    [Tooltip("Drag Elevator_Door_L here")]
    public Transform doorLeft;
    [Tooltip("Drag Elevator_Door_R here")]
    public Transform doorRight;
    [Tooltip("Drag Elevator_Door_L.001 here (second elevator left door)")]
    public Transform doorLeft2;
    [Tooltip("Drag Elevator_Door_R.001 here (second elevator right door)")]
    public Transform doorRight2;

    [Header("Door Settings")]
    [Tooltip("Distance each door moves (in world units)")]
    public float moveDistance = 1.0f;
    [Tooltip("Speed of door movement")]
    public float speed = 2.0f;
    [Tooltip("Should doors start open?")]
    public bool startOpen = true;
    [Tooltip("Show debug messages in console")]
    public bool debugMode = true;

    [Header("Movement Axis")]
    [Tooltip("Which axis do the doors move on? Usually X for left/right")]
    public Vector3 leftDoorMoveDirection = Vector3.right;  // Left door moves right to close
    public Vector3 rightDoorMoveDirection = Vector3.left;  // Right door moves left to close

    private Vector3 leftOpenPos;
    private Vector3 rightOpenPos;
    private Vector3 leftClosedPos;
    private Vector3 rightClosedPos;
    
    private Vector3 leftOpenPos2;
    private Vector3 rightOpenPos2;
    private Vector3 leftClosedPos2;
    private Vector3 rightClosedPos2;

    private bool isOpen = false;
    private bool isMoving = false;

    void Start()
    {
        // Auto-find doors if not assigned
        if (doorLeft == null)
        {
            GameObject found = GameObject.Find("Elevator_Door_L");
            if (found != null)
            {
                doorLeft = found.transform;
                Debug.Log("[ElevatorDoor] Found Elevator_Door_L");
            }
        }
        
        if (doorRight == null)
        {
            GameObject found = GameObject.Find("Elevator_Door_R");
            if (found != null)
            {
                doorRight = found.transform;
                Debug.Log("[ElevatorDoor] Found Elevator_Door_R");
            }
        }
        
        if (doorLeft2 == null)
        {
            GameObject found = GameObject.Find("Elevator_Door_L.001");
            if (found != null)
            {
                doorLeft2 = found.transform;
                Debug.Log("[ElevatorDoor] Found Elevator_Door_L.001");
            }
        }
        
        if (doorRight2 == null)
        {
            GameObject found = GameObject.Find("Elevator_Door_R.001");
            if (found != null)
            {
                doorRight2 = found.transform;
                Debug.Log("[ElevatorDoor] Found Elevator_Door_R.001");
            }
        }

        if (doorLeft == null || doorRight == null)
        {
            Debug.LogError("[ElevatorDoor] DOORS NOT FOUND! Please assign Elevator_Door_L and Elevator_Door_R in Inspector!");
            enabled = false;
            return;
        }
        
        bool hasSecondElevator = (doorLeft2 != null && doorRight2 != null);

        // DEBUG: Afișează structura ușilor
        Debug.Log($"[ElevatorDoor] Left door has {doorLeft.childCount} children");
        for (int i = 0; i < doorLeft.childCount; i++)
        {
            Transform child = doorLeft.GetChild(i);
            Debug.Log($"  - Child {i}: {child.name} (MeshRenderer: {child.GetComponent<MeshRenderer>() != null})");
        }
        
        Debug.Log($"[ElevatorDoor] Right door has {doorRight.childCount} children");
        for (int i = 0; i < doorRight.childCount; i++)
        {
            Transform child = doorRight.GetChild(i);
            Debug.Log($"  - Child {i}: {child.name} (MeshRenderer: {child.GetComponent<MeshRenderer>() != null})");
        }

        // Salvez pozitiile WORLD ca pozitii DESCHISE pentru primul lift
        leftOpenPos = doorLeft.position;
        rightOpenPos = doorRight.position;

        // Pozitiile INCHISE - calculez pe baza directiei de miscare
        leftClosedPos = leftOpenPos + leftDoorMoveDirection.normalized * moveDistance;
        rightClosedPos = rightOpenPos + rightDoorMoveDirection.normalized * moveDistance;

        if (debugMode)
        {
            Debug.Log($"[ElevatorDoor] Left door: Open={leftOpenPos}, Closed={leftClosedPos}");
            Debug.Log($"[ElevatorDoor] Right door: Open={rightOpenPos}, Closed={rightClosedPos}");
        }
        
        // Dacă există al doilea lift, salvez și pozițiile lui
        if (hasSecondElevator)
        {
            leftOpenPos2 = doorLeft2.position;
            rightOpenPos2 = doorRight2.position;
            
            leftClosedPos2 = leftOpenPos2 + leftDoorMoveDirection.normalized * moveDistance;
            rightClosedPos2 = rightOpenPos2 + rightDoorMoveDirection.normalized * moveDistance;
            
            if (debugMode)
            {
                Debug.Log($"[ElevatorDoor] Second elevator - Left door: Open={leftOpenPos2}, Closed={leftClosedPos2}");
                Debug.Log($"[ElevatorDoor] Second elevator - Right door: Open={rightOpenPos2}, Closed={rightClosedPos2}");
            }
        }

        // Setez pozitia initiala
        if (startOpen)
        {
            doorLeft.position = leftOpenPos;
            doorRight.position = rightOpenPos;
            if (hasSecondElevator)
            {
                doorLeft2.position = leftOpenPos2;
                doorRight2.position = rightOpenPos2;
            }
            isOpen = true;
            if (debugMode) Debug.Log("[ElevatorDoor] Doors START OPEN");
        }
        else
        {
            doorLeft.position = leftClosedPos;
            doorRight.position = rightClosedPos;
            if (hasSecondElevator)
            {
                doorLeft2.position = leftClosedPos2;
                doorRight2.position = rightClosedPos2;
            }
            isOpen = false;
            if (debugMode) Debug.Log("[ElevatorDoor] Doors START CLOSED");
        }
    }

    public void OpenDoors()
    {
        if (debugMode) Debug.Log($"[ElevatorDoor] OpenDoors called. isOpen={isOpen}, isMoving={isMoving}");
        
        bool hasSecondElevator = (doorLeft2 != null && doorRight2 != null);
        
        if (!isOpen && !isMoving)
        {
            StartCoroutine(MoveDoors(leftOpenPos, rightOpenPos, leftOpenPos2, rightOpenPos2, true, hasSecondElevator));
        }
    }

    public void CloseDoors()
    {
        if (debugMode) Debug.Log($"[ElevatorDoor] CloseDoors called. isOpen={isOpen}, isMoving={isMoving}");
        
        bool hasSecondElevator = (doorLeft2 != null && doorRight2 != null);
        
        if (isOpen && !isMoving)
        {
            StartCoroutine(MoveDoors(leftClosedPos, rightClosedPos, leftClosedPos2, rightClosedPos2, false, hasSecondElevator));
        }
    }

    public void ActivateElevator()
    {
        if (debugMode) Debug.Log("[ElevatorDoor] ActivateElevator called!");
        
        if (!isMoving)
        {
            StartCoroutine(ElevatorSequence());
        }
    }

    IEnumerator ElevatorSequence()
    {
        if (debugMode) Debug.Log("[ElevatorDoor] Starting elevator sequence...");
        
        bool hasSecondElevator = (doorLeft2 != null && doorRight2 != null);
        
        // Închide ușile dacă sunt deschise
        if (isOpen)
        {
            if (debugMode) Debug.Log("[ElevatorDoor] Closing doors...");
            yield return StartCoroutine(MoveDoors(leftClosedPos, rightClosedPos, leftClosedPos2, rightClosedPos2, false, hasSecondElevator));
        }

        // Așteaptă 2 secunde
        if (debugMode) Debug.Log("[ElevatorDoor] Waiting 2 seconds...");
        yield return new WaitForSeconds(2f);

        // Redeschide ușile
        if (debugMode) Debug.Log("[ElevatorDoor] Opening doors...");
        yield return StartCoroutine(MoveDoors(leftOpenPos, rightOpenPos, leftOpenPos2, rightOpenPos2, true, hasSecondElevator));
        
        if (debugMode) Debug.Log("[ElevatorDoor] Elevator sequence complete!");
    }

    IEnumerator MoveDoors(Vector3 leftTarget, Vector3 rightTarget, Vector3 leftTarget2, Vector3 rightTarget2, bool opening, bool moveSecondElevator)
    {
        isMoving = true;
        
        float elapsed = 0f;
        float duration = moveDistance / speed;
        
        Vector3 leftStart = doorLeft.position;
        Vector3 rightStart = doorRight.position;
        
        Vector3 leftStart2 = moveSecondElevator ? doorLeft2.position : Vector3.zero;
        Vector3 rightStart2 = moveSecondElevator ? doorRight2.position : Vector3.zero;

        if (debugMode) Debug.Log($"[ElevatorDoor] Moving doors from {leftStart} to {leftTarget}");
        if (debugMode && moveSecondElevator) Debug.Log($"[ElevatorDoor] Moving second elevator from {leftStart2} to {leftTarget2}");

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            
            // Smooth interpolation
            t = t * t * (3f - 2f * t);
            
            // USE WORLD POSITION - First elevator
            doorLeft.position = Vector3.Lerp(leftStart, leftTarget, t);
            doorRight.position = Vector3.Lerp(rightStart, rightTarget, t);
            
            // Second elevator if exists
            if (moveSecondElevator)
            {
                doorLeft2.position = Vector3.Lerp(leftStart2, leftTarget2, t);
                doorRight2.position = Vector3.Lerp(rightStart2, rightTarget2, t);
            }

            yield return null;
        }

        // Ensure exact final positions
        doorLeft.position = leftTarget;
        doorRight.position = rightTarget;
        
        if (moveSecondElevator)
        {
            doorLeft2.position = leftTarget2;
            doorRight2.position = rightTarget2;
        }

        isOpen = opening;
        isMoving = false;
        
        if (debugMode) Debug.Log($"[ElevatorDoor] Doors finished moving. isOpen={isOpen}");
    }

    // Pentru testare in Editor
    [ContextMenu("Test Open Doors")]
    public void TestOpen() => OpenDoors();
    
    [ContextMenu("Test Close Doors")]
    public void TestClose() => CloseDoors();
    
    [ContextMenu("Test Elevator Sequence")]
    public void TestSequence() => ActivateElevator();
}
