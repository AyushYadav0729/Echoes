using UnityEngine;

// Filled in. Class/method names and signatures unchanged from the template.

// ---------- Spikes ----------
// Static hazard. No movement, just instant death on touch.
public class Spike : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>()?.Die();
        }
        else if (other.CompareTag("Echo"))
        {
            other.GetComponent<EchoPlayer>()?.Despawn();
        }
    }
}


// ---------- Projectile Laser ----------
// Fires along a fixed path, destroyed on first contact with anything solid.
[RequireComponent(typeof(Collider2D))]
public class ProjectileLaser : MonoBehaviour
{
    public float speed = 4f;
    public Vector2 direction = Vector2.right;

    void FixedUpdate()
    {
        // Fully deterministic — same speed/direction every run, no
        // randomness, since echoes rely on hazards behaving identically.
        transform.position += (Vector3)(direction.normalized * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>()?.Die();
        }
        else if (other.CompareTag("Echo"))
        {
            other.GetComponent<EchoPlayer>()?.Despawn();
        }

        // Consumed on first contact with anything solid (Player, Echo,
        // ground, walls) — this is what lets a body standing in the path
        // protect whatever is behind it.
        Destroy(gameObject);
    }
}

