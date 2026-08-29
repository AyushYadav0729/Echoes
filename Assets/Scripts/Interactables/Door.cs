using UnityEngine;

// Door — opens when its linked HoldPlate is pressed, closes when released.
// Same pattern as StaticLaser: exposes an Open()/Close() pair that
// HoldPlate calls directly.

[RequireComponent(typeof(Collider2D))]
public class Door : MonoBehaviour
{
    [Header("Visual")]
    public SpriteRenderer doorRenderer;
    public Color closedColor = new Color(0.5f, 0.35f, 0.2f, 1f);   // solid brown
    public Color openColor = new Color(0.5f, 0.35f, 0.2f, 0.25f);  // faded/see-through

    private Collider2D doorCollider;

    void Awake()
    {
        doorCollider = GetComponent<Collider2D>();
        if (doorRenderer == null) doorRenderer = GetComponent<SpriteRenderer>();
        Close(); // doors start closed by default
    }

    public void Open()
    {
        doorCollider.enabled = false; // no longer solid — player/echo can pass through
        if (doorRenderer != null)
            doorRenderer.color = openColor;
    }

    public void Close()
    {
        doorCollider.enabled = true; // solid again — blocks movement
        if (doorRenderer != null)
            doorRenderer.color = closedColor;
    }
}