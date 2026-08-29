using UnityEngine;

// PERSON 1 — filled in.
// Class/method names and signatures are unchanged from the template.
// NOTE: uses rb.velocity (Unity 2022.3 LTS). If your project is on
// Unity 6, rename every rb.velocity below to rb.linearVelocity.

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.1f;

    [Header("References — assign in Inspector")]
    public Transform levelStartPoint;
    public DeathEffectSpawner deathEffectSpawner;
    public CharacterAnimator characterAnimator; // lives on a "Visual" child, shared with EchoPlayer
    public EchoManager echoManager;             // owns the 3-echo cap/rotation

    private Rigidbody2D rb;
    private EchoRecorder echoRecorder;
    private bool isGrounded;
    private bool facingRight = true;
    private bool isOnInteractable;
    private float horizontalInput;
    private float attemptStartTime;
    private int attemptNumber;

    // Exposed read-only so EchoRecorder can sample them without a second
    // source of truth for facing/plate state.
    public bool FacingRight => facingRight;
    public bool IsOnInteractable => isOnInteractable;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        echoRecorder = GetComponent<EchoRecorder>();
    }

    void Start()
    {
        BeginAttempt();
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal"); // arrows + A/D by default input map

        if (horizontalInput > 0f) facingRight = true;
        else if (horizontalInput < 0f) facingRight = false;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        if (characterAnimator != null)
            characterAnimator.UpdateFromMovement(horizontalInput, isGrounded);

        if (Input.GetKeyDown(KeyCode.E))
            BankEcho();

        if (Input.GetKeyDown(KeyCode.R))
            RestartLevel();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        isGrounded = groundCheck != null &&
            Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Backup ground flag in case the OverlapCircle above misses a frame.
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
            isGrounded = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hazard"))
        {
            Die();
        }
        else if (other.CompareTag("Goal"))
        {
            GameEvents.RaiseLevelCleared();
        }
        else if (other.CompareTag("Interactable"))
        {
            isOnInteractable = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Interactable"))
            isOnInteractable = false;
    }

    public void Die()
    {

        deathEffectSpawner?.SpawnDeathEffect(transform.position);
        EndAttempt(AttemptEndReason.Died);
    }

    public void BankEcho()
    {
        EndAttempt(AttemptEndReason.EchoButton);
    }

    public void RestartLevel()
    {
        echoManager?.ClearAllEchoes();
        RespawnAtLevelStart();
        BeginAttempt();
    }

    void EndAttempt(AttemptEndReason reason)
    {
        var frames = echoRecorder.StopAndGetRecording();
        echoManager?.AddEcho(frames);

        GameEvents.RaiseAttemptEnded(new AttemptResult
        {
            reason = reason,
            durationSeconds = Time.time - attemptStartTime,
            endPosition = rb.position,
            attemptNumber = attemptNumber
        });

        RespawnAtLevelStart();
        BeginAttempt();
    }

    void RespawnAtLevelStart()
    {
        rb.linearVelocity = Vector2.zero;
        if (levelStartPoint != null)
            transform.position = levelStartPoint.position;
    }

    void BeginAttempt()
    {
        attemptNumber++;
        attemptStartTime = Time.time;
        echoRecorder.ResetRecording();
        echoManager?.SpawnAllEchoesFromLevelStart();
    }
}