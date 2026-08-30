using UnityEngine;

// Filled in. Class/method names and signatures unchanged from the template.
// Implements IInteractable (from Core/) so it works identically whether
// triggered by the live player or an echo.
//
// No color/sprite change — instead, the plate physically moves down when
// pressed and back up when released, like a real mechanical button.

public class HoldPlate : MonoBehaviour, IInteractable
{
    [Header("Linked Object")]
    public GameObject linkedObject; // e.g. a door, or a StaticLaser to disable

    [Header("Press Movement")]
    public float pressDepth = 0.08f; // how far down the plate moves when pressed

    private int pressingCount = 0; // how many things are currently on the plate
    private Vector3 upPosition;
    private Vector3 downPosition;

    void Awake()
    {
        upPosition = transform.localPosition;
        downPosition = upPosition - new Vector3(0f, pressDepth, 0f);
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
        transform.localPosition = downPosition;

        if (linkedObject != null)
        {
            var laser = linkedObject.GetComponent<StaticLaser>();
            laser?.SetActive(false); // holding a plate disables its linked static laser

            var door = linkedObject.GetComponent<Door>();
            door?.Open();

            var platform = linkedObject.GetComponent<MovingPlatform>();
            platform?.Raise();
        }
    }

    public void OnReleased(GameObject presser)
    {
        transform.localPosition = upPosition;

        if (linkedObject != null)
        {
            var laser = linkedObject.GetComponent<StaticLaser>();
            laser?.SetActive(true);

            var door = linkedObject.GetComponent<Door>();
            door?.Close();

            var platform = linkedObject.GetComponent<MovingPlatform>();
            platform?.Lower();
        }
    }
}