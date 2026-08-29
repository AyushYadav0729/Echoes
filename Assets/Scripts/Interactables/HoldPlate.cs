using UnityEngine;

// Filled in. Class/method names and signatures unchanged from the template.
// Implements IInteractable (from Core/) so it works identically whether
// triggered by the live player or an echo.

public class HoldPlate : MonoBehaviour, IInteractable
{
    [Header("Linked Object")]
    public GameObject linkedObject; // e.g. a door, or a StaticLaser to disable

    [Header("Visual")]
    public SpriteRenderer plateRenderer;
    public Color activeColor = new Color(1f, 0.75f, 0.2f, 1f);   // amber
    public Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.4f); // dim outline look

    private int pressingCount = 0; // how many things are currently on the plate

    void Awake()
    {
        if (plateRenderer == null) plateRenderer = GetComponent<SpriteRenderer>();
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
        if (plateRenderer != null)
            plateRenderer.color = activeColor;

        if (linkedObject != null)
        {
            var laser = linkedObject.GetComponent<StaticLaser>();
            laser?.SetActive(false); // holding a plate disables its linked static laser
            
            var door = linkedObject.GetComponent<Door>();
            door?.Open();
        }
    }

    public void OnReleased(GameObject presser)
    {
        if (plateRenderer != null)
            plateRenderer.color = inactiveColor;

        if (linkedObject != null)
        {
            var laser = linkedObject.GetComponent<StaticLaser>();
            laser?.SetActive(true);

            var door = linkedObject.GetComponent<Door>();
            door?.Close();
        }
    }
}