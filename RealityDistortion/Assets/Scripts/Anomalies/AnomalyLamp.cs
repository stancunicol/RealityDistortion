using UnityEngine;

public class AnomalyLamp : MonoBehaviour
{
    [Header("Stages")]
    [SerializeField] private float[] intensities = { 6f, 3f, 1.5f, 2f, 2f, 0f };
    [SerializeField] private float[] volumes = { 0f, 0f, 0.2f, 0.5f, 0.8f, 0.4f };

    [Header("References")]
    [SerializeField] private Light lampLight;
    [SerializeField] private AudioSource lampAudio;
    [SerializeField] private float flickerSpeed = 0.1f;
    [SerializeField] private int currentStage = 0;

    private float flickerTimer = 0f;
    private bool flickerOn = true;

    private void Update()
    {
        if (currentStage == 3 || currentStage == 4)
        {
            flickerTimer += Time.deltaTime;
            if (flickerTimer >= flickerSpeed)
            {
                flickerTimer = 0f;
                flickerOn = !flickerOn;
                lampLight.intensity = flickerOn
                    ? Random.Range(intensities[3] * 0.8f, intensities[3])
                    : Random.Range(intensities[3] * 0.25f, intensities[3] * 0.5f);
            }
        }
    }

    public void AdvanceStage()
    {
        if (currentStage < intensities.Length - 1)
            currentStage++;
        ApplyStage();
    }

    private void ApplyStage()
    {
        lampLight.intensity = intensities[currentStage];
        lampAudio.volume = volumes[currentStage];

        if (volumes[currentStage] > 0f && !lampAudio.isPlaying)
            lampAudio.Play();
        else if (volumes[currentStage] == 0f)
            lampAudio.Stop();

        if (currentStage == 3) flickerSpeed = 0.1f;
        if (currentStage == 4) flickerSpeed = 0.05f;
    }
}
