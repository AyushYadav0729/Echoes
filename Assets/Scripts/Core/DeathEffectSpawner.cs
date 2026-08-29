using UnityEngine;

public class DeathEffectSpawner : MonoBehaviour
{
[Header("Setup — assign in Inspector")]
public GameObject deathBurstPrefab;


public void SpawnDeathEffect(Vector3 position)
{
    if (deathBurstPrefab == null)
        return;

    // Create the particle effect exactly at the death position
    GameObject instance = Instantiate(
        deathBurstPrefab,
        position,
        Quaternion.identity
    );

    // Make sure it has no parent that could move it
    instance.transform.parent = null;

    ParticleSystem ps = instance.GetComponent<ParticleSystem>();

    if (ps != null)
    {
        // Play the particle effect
        ps.Play();

        // Destroy it after the particles have finished
        float lifetime =
            ps.main.duration +
            ps.main.startLifetime.constantMax;

        Destroy(instance, lifetime);
    }
    else
    {
        // Fallback cleanup
        Destroy(instance, 1f);
    }
}

}
