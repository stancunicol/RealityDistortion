using UnityEngine;

public class LampTrigger : MonoBehaviour
{
    [SerializeField]private AnomalyLamp anomalyLamp;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggered)
        {
            anomalyLamp.AdvanceStage();
            triggered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggered = false;
        }
    }
}
