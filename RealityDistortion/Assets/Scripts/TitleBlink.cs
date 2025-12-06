using UnityEngine;
using TMPro;
using System.Collections;

public class TitleBlink : MonoBehaviour
{
    public TMP_Text text;

    [Header("Fade settings")]
    public float fadeSpeedMin = 0.5f;
    public float fadeSpeedMax = 2f;

    [Header("Timings")]
    public float minPause = 0.2f;
    public float maxPause = 0.8f;

    void Start()
    {
        StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            // Decide aleator ce se întâmplă în acest ciclu
            bool fullDisappear = Random.value < 0.5f;  // 50% șansă să dispară complet

            if (fullDisappear)
            {
                Dispariție completă (enable off)
                yield return StartCoroutine(FadeTo(0f)); // se estompează întâi
                text.enabled = false;

                yield return new WaitForSeconds(Random.Range(minPause, maxPause));

                text.enabled = true;
                yield return StartCoroutine(FadeTo(1f)); // revine cu fade
            }
            else
            {
                // Doar scade opacitatea (fade parțial)
                float randomTarget = Random.Range(0.2f, 0.8f); // un nivel aleator de opacitate
                yield return StartCoroutine(FadeTo(randomTarget));

                yield return new WaitForSeconds(Random.Range(minPause, maxPause));

                yield return StartCoroutine(FadeTo(1f)); // revine la vizibil
            }

            // Pauză înainte de următorul ciclu
            yield return new WaitForSeconds(Random.Range(minPause, maxPause));
        }
    }

    IEnumerator FadeTo(float targetAlpha)
    {
        float speed = Random.Range(fadeSpeedMin, fadeSpeedMax);

        while (Mathf.Abs(text.color.a - targetAlpha) > 0.01f)
        {
            Color c = text.color;
            c.a = Mathf.MoveTowards(c.a, targetAlpha, Time.deltaTime * speed);
            text.color = c;
            yield return null;
        }
    }
}
