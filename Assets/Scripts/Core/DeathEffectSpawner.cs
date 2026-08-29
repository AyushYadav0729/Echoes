using UnityEngine;

// Spawns the death particle burst at a given position, then cleans itself
// up automatically once the effect finishes playing.
//
// Attach this to any persistent object in the scene (e.g. your
// LevelManager), or make it a static helper — either works. Shown here
// as a simple MonoBehaviour with a public method other scripts call.

public class DeathEffectSpawner : MonoBehaviour
{
    [Header("Setup — assign in Inspector")]
    public GameObject deathBurstPrefab;

    public void SpawnDeathEffect(Vector3 position)
    {
        if (deathBurstPrefab == null) return;

        GameObject instance = Instantiate(deathBurstPrefab, position, Quaternion.identity);
        ParticleSystem ps = instance.GetComponent<ParticleSystem>();

        if (ps != null)
        {
            ps.Play();
            Destroy(instance, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
        {
            Destroy(instance, 1f); // fallback cleanup if something's misconfigured
        }
    }
}
