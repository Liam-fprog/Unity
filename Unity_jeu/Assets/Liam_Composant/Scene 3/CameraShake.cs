using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Réglages")]
    public float amplitude = 0.05f;
    public float frequency = 25f;
    public float smooth = 5f;

    private Vector3 initialPos;
    private bool shaking = false;
    private float shakeTime = 0f;
    private float baseAmplitude;

    void Start()
    {
        initialPos = transform.localPosition;
        baseAmplitude = amplitude;
    }

    void Update()
    {
        if (shaking)
        {
            shakeTime += Time.deltaTime * frequency;
            float offsetX = Mathf.PerlinNoise(shakeTime, 0f) * 2f - 1f;
            float offsetY = Mathf.PerlinNoise(0f, shakeTime) * 2f - 1f;

            Vector3 shakeOffset = new Vector3(offsetX, offsetY, 0f) * amplitude;
            transform.localPosition = initialPos + shakeOffset;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPos, Time.deltaTime * smooth);
        }
    }

    public void SetShaking(bool active)
    {
        shaking = active;
        if (!active) shakeTime = 0f;
    }

    public void SetShaking(bool active, float intensity)
    {
        shaking = active;
        amplitude = baseAmplitude * Mathf.Clamp01(intensity);
        if (!active) shakeTime = 0f;
    }
}
