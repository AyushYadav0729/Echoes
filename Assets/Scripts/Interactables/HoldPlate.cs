using UnityEngine;
using System.Collections.Generic;

// Filled in. Class/method names and signatures unchanged from the template.
// Implements IInteractable (from Core/) so it works identically whether
// triggered by the live player or an echo.
//
// Uses a HashSet of actual colliders instead of a simple counter, since
// Unity does NOT fire OnTriggerExit2D when a collider is destroyed (e.g.
// an echo despawning while still standing on the plate) — a counter would
// get stuck. Update() cleans out destroyed/null entries every frame.

public class HoldPlate : MonoBehaviour, IInteractable
{
    [Header("Linked Object")]
    public GameObject linkedObject; // e.g. a door, or a StaticLaser to disable

    [Header("Visual — Sprites")]
    public SpriteRenderer plateRenderer;
    public Sprite pressedSprite;
    public Sprite unpressedSprite;

    [Header("Visual — Optional Tint")]
    public Color activeColor = new Color(1f, 0.75f, 0.2f, 1f);   // amber
    public Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.4f); // dim outline look

    private HashSet<Collider2D> pressers = new HashSet<Collider2D>();
    private bool wasPressed = false;

    void Awake()
    {
        if (plateRenderer == null) plateRenderer = GetComponent<SpriteRenderer>();

        // Guarantee correct starting appearance regardless of what was
        // left in the Editor.
        if (plateRenderer != null)
        {
            if (unpressedSprite != null) plateRenderer.sprite = unpressedSprite;
            plateRenderer.color = inactiveColor;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Echo")) return;
        pressers.Add(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        pressers.Remove(other);
    }

    void Update()
    {
        // Clean out any colliders destroyed without firing OnTriggerExit2D
        // (e.g. an echo despawning mid-press).
        pressers.RemoveWhere(c => c == null);

        bool isPressed = pressers.Count > 0;

        if (isPressed && !wasPressed)
            OnPressed(null);
        else if (!isPressed && wasPressed)
            OnReleased(null);

        wasPressed = isPressed;
    }

    public void OnPressed(GameObject presser)
    {
        if (plateRenderer != null)
        {
            if (pressedSprite != null) plateRenderer.sprite = pressedSprite;
            plateRenderer.color = activeColor;
        }

        if (linkedObject != null)
        {
            var toggleable = linkedObject.GetComponent<IToggleable>();
            toggleable?.SetActive(true); // pressing activates the linked object
        }
    }

    public void OnReleased(GameObject presser)
    {
        if (plateRenderer != null)
        {
            if (unpressedSprite != null) plateRenderer.sprite = unpressedSprite;
            plateRenderer.color = inactiveColor;
        }

        if (linkedObject != null)
        {
            var toggleable = linkedObject.GetComponent<IToggleable>();
            toggleable?.SetActive(false); // releasing deactivates the linked object
        }
    }
}