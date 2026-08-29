using UnityEngine;

// PERSON 2 — fill in the logic inside each method.

// ---------- Spikes ----------
// Static hazard. No movement, just instant death on touch.
public class Spike : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // TODO: check other.CompareTag("Player") -> call PlayerController.Die()
        // TODO: check other.CompareTag("Echo") -> despawn that echo (it "dies" here on replay)
    }
}


// ---------- Projectile Laser ----------
// Fires along a fixed path, destroyed on first contact with anything solid.
public class ProjectileLaser : MonoBehaviour
{
    public float speed = 4f;
    public Vector2 direction = Vector2.right;

    void FixedUpdate()
    {
        // TODO: move along `direction` at `speed` — must be fully deterministic,
        // no randomness, since echoes rely on hazards behaving identically every run
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // TODO: if other is Player -> Die(); if Echo -> despawn it
        // TODO: either way, destroy this projectile (Destroy(gameObject)) —
        // it's consumed on first contact, protecting anything behind it
    }
}


// ---------- Static Laser ----------
// Continuous beam. Cannot be blocked by standing in it — only disabled via a linked plate.
public class StaticLaser : MonoBehaviour
{
    private bool isActive = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;
        // TODO: if other is Player -> Die(); if Echo -> despawn it
        // Note: unlike ProjectileLaser, do NOT destroy this object — it stays and keeps firing
    }

    // Called by the linked HoldPlate via OnPressed/OnReleased
    public void SetActive(bool active)
    {
        isActive = active;
        // TODO: visual change — fade to near-invisible/gray when inactive
    }
}
