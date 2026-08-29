using UnityEngine;

// PERSON 2 — fill in the logic inside each method.
// Implements IInteractable (from Core/) so it works identically
// whether triggered by the live player or an echo.

public class HoldPlate : MonoBehaviour, IInteractable
{
    [Header("Linked Object")]
    public GameObject linkedObject; // e.g. a door, or a StaticLaser to disable

    private int pressingCount = 0; // how many things are currently on the plate

    void OnTriggerEnter2D(Collider2D other)
    {
        // TODO: check other.CompareTag("Player") or other.CompareTag("Echo")
        // TODO: pressingCount++, if this is the first presser call OnPressed(other.gameObject)
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // TODO: pressingCount--, if pressingCount reaches 0 call OnReleased(other.gameObject)
    }

    public void OnPressed(GameObject presser)
    {
        // TODO: visual state change (bright fill + press animation)
        // TODO: activate linkedObject (e.g. disable a StaticLaser, open a door)
    }

    public void OnReleased(GameObject presser)
    {
        // TODO: visual state change back to dim/inactive
        // TODO: deactivate linkedObject
    }
}
