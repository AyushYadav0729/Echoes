using UnityEngine;

// A laser that shrinks away (scales to zero) when its linked HoldPlate is
// pressed, and grows back to full size when released. Uses your existing
// laser sprite directly — no separate beam-stretching math, no
// startPoint/endPoint setup needed.

public class StaticLaser : MonoBehaviour, IToggleable  
{
    [Header("Scale Speed")]
    public float scaleSpeed = 40f; // higher = faster shrink/grow

    private Vector3 fullScale;
    private bool isActive = true;
    private Collider2D col;

    void Awake()
    {
        fullScale = transform.localScale; // whatever size you set it to in the Editor is "full on"
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        Vector3 target = isActive ? fullScale : Vector3.zero;
        transform.localScale = Vector3.MoveTowards(transform.localScale, target, scaleSpeed * Time.deltaTime);

        // Disable the collider once it's basically invisible, so a
        // shrunk-to-nothing laser can't still kill you.
        if (col != null)
            col.enabled = transform.localScale.magnitude > 0.05f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<PlayerController>()?.Die();
        else if (other.CompareTag("Echo"))
            other.GetComponent<EchoPlayer>()?.Despawn();
    }

    // Called by HoldPlate — true = laser on (full size), false = laser off (shrinks to nothing)
    public void SetActive(bool active)
    {
        isActive = !active; // inverted: "pressed" (true) means the laser turns OFF
    }
}