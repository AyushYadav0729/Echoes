using UnityEngine;

// Door — opens when its linked HoldPlate is pressed, closes when released.
// Same pattern as StaticLaser: exposes an Open()/Close() pair that
// HoldPlate calls directly.
//
// Sprite-swap only — no color tint.

[RequireComponent(typeof(Collider2D))]
public class Door : MonoBehaviour
{
    [Header("Visual — Sprites")]
    public Sprite closedSprite;
    public Sprite openSprite;

    public SpriteRenderer doorRenderer;

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

        if (doorRenderer != null && openSprite != null)
            doorRenderer.sprite = openSprite;
    }

    public void Close()
    {
        doorCollider.enabled = true; // solid again — blocks movement

        if (doorRenderer != null && closedSprite != null)
            doorRenderer.sprite = closedSprite;
    }
}