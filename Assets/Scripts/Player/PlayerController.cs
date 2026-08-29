using System.Collections;
using UnityEngine;

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
    public CharacterAnimator characterAnimator;
    public EchoManager echoManager;

    [Header("Respawn Settings")]
    public float respawnProtectionTime = 0.15f;

    private Rigidbody2D rb;
    private EchoRecorder echoRecorder;

    private bool isGrounded;
    private bool facingRight = true;
    private bool isOnInteractable;

    private bool isDying = false;
    private bool respawnProtection = false;

    private float horizontalInput;
    private float attemptStartTime;
    private int attemptNumber;

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
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput > 0f)
            facingRight = true;
        else if (horizontalInput < 0f)
            facingRight = false;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isDying)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );
        }

        if (characterAnimator != null)
        {
            characterAnimator.UpdateFromMovement(
                horizontalInput,
                isGrounded
            );
        }

        if (Input.GetKeyDown(KeyCode.E) && !isDying)
        {
            BankEcho();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }

    void FixedUpdate()
    {
        if (!isDying)
        {
            rb.linearVelocity = new Vector2(
                horizontalInput * moveSpeed,
                rb.linearVelocity.y
            );
        }

        isGrounded = groundCheck != null &&
            Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore everything while dying or during respawn protection
        if (isDying || respawnProtection)
            return;

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
        {
            isOnInteractable = false;
        }
    }

    public void Die()
    {
        // Make absolutely sure Die() can only happen once
        if (isDying || respawnProtection)
            return;

        isDying = true;

        // IMPORTANT:
        // Capture the position BEFORE respawning.
        Vector3 deathPosition = transform.position;

        // Spawn particle at the actual death location.
        if (deathEffectSpawner != null)
        {
            deathEffectSpawner.SpawnDeathEffect(deathPosition);
        }

        // End the attempt and respawn.
        EndAttempt(AttemptEndReason.Died);
    }

    public void BankEcho()
    {
        if (isDying)
            return;

        EndAttempt(AttemptEndReason.EchoButton);
    }

    public void RestartLevel()
    {
        echoManager?.ClearAllEchoes();

        RespawnAtLevelStart();
        BeginAttempt();

        StartCoroutine(RespawnProtection());
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

        // Protect player from immediately triggering
        // the same hazard again at the respawn point.
        StartCoroutine(RespawnProtection());
    }

    void RespawnAtLevelStart()
    {
        rb.linearVelocity = Vector2.zero;

        if (levelStartPoint != null)
        {
            transform.position = levelStartPoint.position;
        }
    }

    IEnumerator RespawnProtection()
    {
        respawnProtection = true;

        // Wait a tiny amount of time so Unity can
        // finish processing the teleport/trigger events.
        yield return new WaitForSeconds(respawnProtectionTime);

        respawnProtection = false;
        isDying = false;
    }

    void BeginAttempt()
    {
        attemptNumber++;
        attemptStartTime = Time.time;

        echoRecorder.ResetRecording();

        echoManager?.SpawnAllEchoesFromLevelStart();
    }
}