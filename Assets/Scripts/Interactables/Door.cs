using UnityEngine;

// Door — opens when its linked HoldPlate is pressed, closes when released.
// Same pattern as StaticLaser: exposes an Open()/Close() pair, and
// implements IToggleable so HoldPlate can actually find and call it
// (this was the missing piece — HoldPlate looks for IToggleable
// specifically, not Open()/Close() directly).
//
// Sprite-swap only — no color tint.

[RequireComponent(typeof(Collider2D))]
public class Door : MonoBehaviour, IToggleable
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

    // IToggleable — called by a linked HoldPlate.
    public void SetActive(bool active)
    {
        if (active) Open();
        else Close();
    }
}