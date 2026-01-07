using UnityEngine;
using System.Collections;

public class FootstepZoneDirect : MonoBehaviour
{
    public AudioClip leftClip;
    public AudioClip rightClip;
    public int repeatCount = 3;
    public float interval = 0.3f;
    public float triggerRadius = 1f;

    private bool isPlaying = false;

    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, triggerRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player") && !isPlaying)
            {
                StartCoroutine(PlayFootsteps(hit.transform.position));
            }
        }
    }

    private IEnumerator PlayFootsteps(Vector3 position)
    {
        isPlaying = true;

        for (int i = 0; i < repeatCount; i++)
        {
            AudioSource.PlayClipAtPoint(leftClip, position);
            yield return new WaitForSeconds(interval);
            AudioSource.PlayClipAtPoint(rightClip, position);
            yield return new WaitForSeconds(interval);
        }

        isPlaying = false;
    }
    
    public void ResetAnomaly()
    {
        StopAllCoroutines();
        isPlaying = false;
    }
}
