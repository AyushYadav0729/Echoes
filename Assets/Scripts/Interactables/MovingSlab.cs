using System.Collections.Generic;
using UnityEngine;

// A platform that only moves while its linked plate is held down — moves
// toward its end position at a fixed speed, and freezes exactly where it
// is the instant the plate is released (no automatic return to start).
// Axis, travel distance, and speed are all set per-instance in the
// Inspector, so one script covers every moving-slab placement.
//
// UPDATED: resets to its starting position whenever a new echo is created
// (GameEvents.OnAttemptEnded) or the level is restarted
// (GameEvents.OnLevelReset) — so every attempt begins from a consistent,
// predictable slab position.
//
// Setup: link this object from a HoldPlate's "Linked Object" field, same
// as you'd link a Laser — HoldPlate calls SetActive() through IToggleable.
[RequireComponent(typeof(Rigidbody2D))]
public class MovingSlab : MonoBehaviour, IToggleable
{
    public enum Axis { Horizontal, Vertical }

    [Header("Movement")]
    public Axis moveAxis = Axis.Horizontal;
    public float travelDistance = 3f;  // how far it can move from its starting position
    public float speed = 2f;           // units per second, while the plate is held

    private Rigidbody2D rb;
    private Vector2 startPos;
    private Vector2 axisDir;
    private float currentOffset = 0f; // 0 = at start position, travelDistance = fully extended
    private bool movingForward = true; // which direction it's currently bouncing
    private bool isMoving = false;
    private readonly HashSet<Rigidbody2D> passengers = new HashSet<Rigidbody2D>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; // moved manually, not by physics forces
        startPos = rb.position;
        axisDir = moveAxis == Axis.Horizontal ? Vector2.right : Vector2.up;
    }

    void OnEnable()
    {
        GameEvents.OnAttemptEnded += HandleAttemptEnded;
        GameEvents.OnLevelReset += HandleLevelReset;
    }

    void OnDisable()
    {
        GameEvents.OnAttemptEnded -= HandleAttemptEnded;
        GameEvents.OnLevelReset -= HandleLevelReset;
    }

    private void HandleAttemptEnded(AttemptResult result)
    {
        ResetToStart();
    }

    private void HandleLevelReset()
    {
        ResetToStart();
    }

    public void ResetToStart()
    {
        currentOffset = 0f;
        movingForward = true;
        isMoving = false; // freeze until its plate is pressed again
        passengers.Clear();
        rb.position = startPos;
    }

    // IToggleable — called by a linked HoldPlate. active = plate currently held.
    public void SetActive(bool active)
    {
        isMoving = active; // false = freeze immediately, exactly where it is
    }

    void FixedUpdate()
    {
        if (!isMoving) return; // freezes exactly here — currentOffset/movingForward stay as-is, so it resumes correctly if re-activated

        Vector2 previousPos = rb.position;

        float step = speed * Time.fixedDeltaTime;
        if (movingForward)
        {
            currentOffset += step;
            if (currentOffset >= travelDistance)
            {
                currentOffset = travelDistance;
                movingForward = false; // bounce back
            }
        }
        else
        {
            currentOffset -= step;
            if (currentOffset <= 0f)
            {
                currentOffset = 0f;
                movingForward = true; // bounce forward again
            }
        }

        Vector2 newPos = startPos + axisDir * currentOffset;
        rb.MovePosition(newPos);

        // Carry along anything standing on top.
        Vector2 delta = newPos - previousPos;
        if (delta != Vector2.zero)
        {
            foreach (var passenger in passengers)
                if (passenger != null)
                    passenger.position += delta;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Echo")) return;
        if (IsStandingOnTop(collision))
            passengers.Add(collision.rigidbody);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        passengers.Remove(collision.rigidbody);
    }

    // Simple approximation: treat anything whose center is above this
    // platform's center as "on top" rather than touching from the side.
    bool IsStandingOnTop(Collision2D collision)
    {
        return collision.transform.position.y > transform.position.y;
    }
}