using UnityEngine;

// A crusher that moves down to smash, pauses, retracts back up, pauses,
// and repeats — fully deterministic (same timing every single run), so
// echoes replaying against it behave identically every time.
//
// Setup: position this at its RESTING (up) position in the Editor —
// that's automatically captured as the top of its travel range.

[RequireComponent(typeof(Rigidbody2D))]
public class Crusher : MonoBehaviour
{
    [Header("Movement")]
    public float crushDistance = 2f;   // how far down it travels
    public float crushSpeed = 6f;      // units/sec moving down (usually fast)
    public float retractSpeed = 2f;    // units/sec moving back up (usually slower)

    [Header("Timing")]
    public float pauseAtBottom = 0.3f;
    public float pauseAtTop = 1f;

    private enum State { WaitingAtTop, Crushing, WaitingAtBottom, Retracting }
    private State state = State.WaitingAtTop;

    private Rigidbody2D rb;
    private Vector2 topPos;
    private Vector2 bottomPos;
    private float timer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        topPos = rb.position;
        bottomPos = topPos - new Vector2(0f, crushDistance);
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case State.WaitingAtTop:
                timer += Time.fixedDeltaTime;
                if (timer >= pauseAtTop)
                {
                    timer = 0f;
                    state = State.Crushing;
                }
                break;

            case State.Crushing:
                Vector2 newPos = Vector2.MoveTowards(rb.position, bottomPos, crushSpeed * Time.fixedDeltaTime);
                rb.MovePosition(newPos);
                if (Vector2.Distance(newPos, bottomPos) < 0.01f)
                {
                    state = State.WaitingAtBottom;
                    timer = 0f;
                }
                break;

            case State.WaitingAtBottom:
                timer += Time.fixedDeltaTime;
                if (timer >= pauseAtBottom)
                {
                    timer = 0f;
                    state = State.Retracting;
                }
                break;

            case State.Retracting:
                Vector2 retractPos = Vector2.MoveTowards(rb.position, topPos, retractSpeed * Time.fixedDeltaTime);
                rb.MovePosition(retractPos);
                if (Vector2.Distance(retractPos, topPos) < 0.01f)
                {
                    state = State.WaitingAtTop;
                    timer = 0f;
                }
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<PlayerController>()?.Die();
        else if (other.CompareTag("Echo"))
            other.GetComponent<EchoPlayer>()?.Despawn();
    }
}