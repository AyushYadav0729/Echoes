using UnityEngine;

// PERSON 1 — fill in the logic inside each method.
// Do not rename the class, methods, or change their signatures —
// the integrator (Unity) depends on these exact names to wire things up.

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool facingRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // TODO: cache any other components you need here (e.g. Animator on the Visual child)
    }

    void Update()
    {
        // TODO: read input here (Input.GetAxisRaw("Horizontal"), Input.GetKeyDown(KeyCode.Space))
        // TODO: update facingRight based on movement direction
        // TODO: trigger squash/stretch + eyebrow-direction animation based on movement
    }

    void FixedUpdate()
    {
        // TODO: apply movement to rb.velocity here
        // Keep all physics movement in FixedUpdate, not Update —
        // this matters because EchoRecorder also samples in FixedUpdate,
        // and recording/movement need to stay in sync.
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO: set isGrounded = true if colliding with ground layer
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // TODO: check other.CompareTag("Hazard") -> call Die()
        // TODO: check other.CompareTag("Goal") -> call GameEvents.RaiseLevelCleared()
    }

    public void Die()
    {
        // TODO: stop current recording, save it as AttemptResult with reason = Died
        // TODO: call GameEvents.RaiseAttemptEnded(...)
        // TODO: respawn player at level start
    }

    public void BankEcho()
    {
        // Called when ECHO button (E key) is pressed
        // TODO: stop current recording, save as AttemptResult with reason = EchoButton
        // TODO: respawn player at level start
    }

    public void RestartLevel()
    {
        // Called when RESTART button (R key) is pressed
        // TODO: clear ALL active echoes, respawn player at level start
    }
}
