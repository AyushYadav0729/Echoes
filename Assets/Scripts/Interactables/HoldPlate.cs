using UnityEngine;

// Filled in. Class/method names and signatures unchanged from the template.
// Implements IInteractable (from Core/) so it works identically whether
// triggered by the live player or an echo.
//
// Sprite-swap only — nothing physically moves, so there's no risk of the
// trigger collider shifting and causing enter/exit flicker.

public class HoldPlate : MonoBehaviour, IInteractable
{
    [Header("Linked Object")]
    public GameObject linkedObject; // e.g. a door, or a StaticLaser to disable

    [Header("Visual — Sprites")]
    public SpriteRenderer plateRenderer;
    public Sprite pressedSprite;
    public Sprite unpressedSprite;

    private int pressingCount = 0;

    void Awake()
    {
        if (plateRenderer == null) plateRenderer = GetComponent<SpriteRenderer>();

        if (plateRenderer != null && unpressedSprite != null)
            plateRenderer.sprite = unpressedSprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Echo")) return;

        pressingCount++;
        if (pressingCount == 1)
            OnPressed(other.gameObject);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") && !other.CompareTag("Echo")) return;

        pressingCount = Mathf.Max(0, pressingCount - 1);
        if (pressingCount == 0)
            OnReleased(other.gameObject);
    }

    public void OnPressed(GameObject presser)
    {
        if (plateRenderer != null && pressedSprite != null)
            plateRenderer.sprite = pressedSprite;

        if (linkedObject != null)
        {
            var laser = linkedObject.GetComponent<StaticLaser>();
            laser?.SetActive(false);

            var door = linkedObject.GetComponent<Door>();
            door?.Open();
        }
    }

    public void OnReleased(GameObject presser)
    {
        if (plateRenderer != null && unpressedSprite != null)
            plateRenderer.sprite = unpressedSprite;

        if (linkedObject != null)
        {
            var laser = linkedObject.GetComponent<StaticLaser>();
            laser?.SetActive(true);

            var door = linkedObject.GetComponent<Door>();
            door?.Close();
        }
    }
}