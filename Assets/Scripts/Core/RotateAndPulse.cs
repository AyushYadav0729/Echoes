using UnityEngine;

// Rotates continuously while also pulsing scale up and down in a loop.
// Attach to any GameObject you want animated this way.

public class RotateAndPulse : MonoBehaviour
{
    [Header("Rotation")]
    public float degreesPerSecond = 90f; // positive = counter-clockwise, negative = clockwise

    [Header("Scale Pulse")]
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    public float pulseSpeed = 2f; // higher = faster pulse cycle

    private Vector3 baseScale;

    void Awake()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        // Rotation
        transform.Rotate(0f, 0f, degreesPerSecond * Time.deltaTime);

        // Scale pulse using a sine wave for smooth back-and-forth looping
        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f; // remaps -1..1 to 0..1
        float scale = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = baseScale * scale;
    }
}
