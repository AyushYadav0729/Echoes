using UnityEngine;

public class ScalePulse : MonoBehaviour
{
    [Header("Scale Settings")]
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
    public float speed = 2f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Goes smoothly between 0 and 1 repeatedly
        float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f;

        // Calculate the scale multiplier
        float scaleMultiplier = Mathf.Lerp(minScale, maxScale, t);

        // Change only the scale, not the position
        transform.localScale = originalScale * scaleMultiplier;
    }
}