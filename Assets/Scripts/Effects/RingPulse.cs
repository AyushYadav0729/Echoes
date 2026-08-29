using UnityEngine;

// A ring that expands outward and fades to nothing — used as a visual cue
// for "an echo was just created." Attach to a sprite object with a
// ring/circle-outline texture (a simple circle sprite works fine too).

public class RingPulse : MonoBehaviour
{
    [Header("Animation")]
    public float duration = 0.5f;
    public float startScale = 0.2f;
    public float endScale = 1.5f;

    private SpriteRenderer sr;
    private float timer;
    private Color baseColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
        transform.localScale = Vector3.one * startScale;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);

        float scale = Mathf.Lerp(startScale, endScale, t);
        transform.localScale = Vector3.one * scale;

        Color c = baseColor;
        c.a = Mathf.Lerp(baseColor.a, 0f, t); // fade out as it expands
        sr.color = c;

        if (t >= 1f)
            Destroy(gameObject);
    }
}
