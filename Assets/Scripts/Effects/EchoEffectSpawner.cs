using UnityEngine;

// Spawns the two halves of the echo teleport effect:
// - Expand ring: plays where the attempt ended (player "sucked away")
// - Shrink ring: plays at the respawn point (player "materializes")
// Both use the same RingPulse script, just with different start/end
// scale values set on each prefab.

public class EchoEffectSpawner : MonoBehaviour
{
    [Header("Setup — assign in Inspector")]
    public GameObject expandRingPrefab;
    public GameObject shrinkRingPrefab;

    public void SpawnExpandRing(Vector3 position)
    {
        if (expandRingPrefab == null) return;
        Instantiate(expandRingPrefab, position, Quaternion.identity);
    }

    public void SpawnShrinkRing(Vector3 position)
    {
        if (shrinkRingPrefab == null) return;
        Instantiate(shrinkRingPrefab, position, Quaternion.identity);
    }
}