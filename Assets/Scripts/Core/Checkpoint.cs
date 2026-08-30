using UnityEngine;

// A trigger that updates PlayerController's spawn point when the player
// touches it. Place several of these through your big level.

public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null)
            pc.SetCheckpoint(transform);
    }
}
