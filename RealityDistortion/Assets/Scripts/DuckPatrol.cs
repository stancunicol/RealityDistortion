using UnityEngine;

public class DuckPatrol : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;
    public float rotationSpeed = 4f;

    public GameObject duckModel; // 🎯 ADĂUGAT

    private int currentIndex = 0;
    private bool goingForward = true;

    void Start()
    {
        // Activăm modelul dacă este dezactivat in Inspector
        if (duckModel != null && !duckModel.activeSelf)
        {
            duckModel.SetActive(true);
        }
    }

    void Update()
    {
        if (duckModel == null || !duckModel.activeSelf)
            return; // dacă e off, nu face nimic

        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentIndex];
        Vector3 direction = (target.position - transform.position).normalized;

        // Rotește rata spre waypoint
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        // Mergi înainte
        transform.position += transform.forward * speed * Time.deltaTime;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance < 0.2f)
        {
            if (goingForward)
            {
                currentIndex++;
                if (currentIndex >= waypoints.Length)
                {
                    currentIndex = waypoints.Length - 2;
                    goingForward = false;
                }
            }
            else
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = 1;
                    goingForward = true;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.DrawSphere(waypoints[i].position, 0.2f);
            }

            if (i < waypoints.Length - 1 && waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
    }
}
