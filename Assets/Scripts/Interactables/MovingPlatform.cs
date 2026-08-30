using UnityEngine;

// MovingPlatform — moves between a lowered and raised position when
// triggered by a linked HoldPlate. Same pattern as Door (Open/Close),
// but here we call it Raise/Lower.

public class MovingPlatform : MonoBehaviour
{
    [Header("Positions")]
    public Vector3 raisedOffset = new Vector3(0f, 3f, 0f); // how far up it rises
    public float moveSpeed = 3f;

    private Vector3 loweredPosition;
    private Vector3 targetPosition;

    void Awake()
    {
        loweredPosition = transform.position; // wherever you place it in the Scene = "lowered"
        targetPosition = loweredPosition;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    public void Raise()
    {
        targetPosition = loweredPosition + raisedOffset;
    }

    public void Lower()
    {
        targetPosition = loweredPosition;
    }
}