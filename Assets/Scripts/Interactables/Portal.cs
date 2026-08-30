using System.Collections.Generic;
using UnityEngine;

// A pair of connected portals — touching one teleports Player/Echo to the
// other. Fully deterministic: an echo's recorded frames already contain
// the position jump from when it happened live, so replay reproduces the
// teleport automatically with no special-case logic needed in EchoPlayer.
//
// Setup: place two Portal objects in the level, drag each into the
// other's Linked Portal field. Collider2D must be set to Trigger.
[RequireComponent(typeof(Collider2D))]
public class Portal : MonoBehaviour
{
    [Header("Link")]
    public Portal linkedPortal; // the other portal in this pair

    [Header("Settings")]
    public float teleportCooldown = 0.5f; // prevents an instant back-and-forth loop
    public Vector2 exitOffset = Vector2.zero; // optional nudge away from the linked portal on arrival

    // Tracks objects this portal just received, so they don't immediately
    // trigger a return trip back through the portal they arrived at.
    private readonly Dictionary<GameObject, float> recentlyArrived = new Dictionary<GameObject, float>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (linkedPortal == null) return;
        if (!other.CompareTag("Player") && !other.CompareTag("Echo")) return;

        if (recentlyArrived.TryGetValue(other.gameObject, out float until) && Time.time < until)
            return;

        Vector3 destination = linkedPortal.transform.position + (Vector3)linkedPortal.exitOffset;

        var rb = other.attachedRigidbody;
        if (rb != null)
            rb.position = destination;
        else
            other.transform.position = destination;

        // Mark on the DESTINATION portal so it doesn't immediately send
        // this object right back through.
        linkedPortal.recentlyArrived[other.gameObject] = Time.time + teleportCooldown;
    }
}
