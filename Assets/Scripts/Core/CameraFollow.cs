using UnityEngine;

// Minimal camera follow — tracks the target's X (and optionally Y)
// position with simple smoothing. No bounds/clamping — add later if time allows.

public class CameraFollow : MonoBehaviour
{
    public Transform target; // drag your Player here
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10f); // keep Z at -10 for 2D
    public bool followY = false; // usually false for a side-scroller with flat ground

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(
            target.position.x + offset.x,
            followY ? target.position.y + offset.y : transform.position.y,
            offset.z
        );

        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
    }
}
