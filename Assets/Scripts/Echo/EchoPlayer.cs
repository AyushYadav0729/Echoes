using System.Collections.Generic;
using UnityEngine;

// PERSON 1 — filled in. Class/method names and signatures unchanged.
// UPDATED: uses rb.MovePosition instead of setting transform.position
// directly, so the Kinematic Rigidbody2D's Interpolate setting can smooth
// movement between FixedUpdate ticks — otherwise echoes visually snap
// between recorded points and look choppy compared to the live player.

[RequireComponent(typeof(Rigidbody2D))]
public class EchoPlayer : MonoBehaviour
{
    [Header("References — assign in Inspector")]
    public CharacterAnimator characterAnimator; // same shared component the player uses
    public TrailRenderer trail;                  // onion-skin trail — echoes only

    private Rigidbody2D rb;
    private List<EchoFrame> frames;
    private int currentFrameIndex;
    private float playbackTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(List<EchoFrame> recording)
    {
        frames = recording;
        currentFrameIndex = 0;
        playbackTimer = 0f;

        if (frames != null && frames.Count > 0)
            rb.position = frames[0].position; // snap immediately on spawn, no interpolation needed here

        if (trail != null)
            trail.Clear();
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (frames == null || frames.Count == 0 || currentFrameIndex >= frames.Count)
        {
            // Recording finished — echo simply stops existing, no special state.
            Destroy(gameObject);
            return;
        }

        // Drive playback off elapsed time vs. each frame's recorded timestamp,
        // rather than one array index per FixedUpdate tick — that naive
        // approach only stays in sync if sampleInterval exactly equals the
        // Fixed Timestep, and silently speeds up/slows down otherwise.
        playbackTimer += Time.fixedDeltaTime;

        while (currentFrameIndex < frames.Count - 1 &&
               frames[currentFrameIndex + 1].timestamp <= playbackTimer)
        {
            currentFrameIndex++;
        }

        EchoFrame frame = frames[currentFrameIndex];
        rb.MovePosition(frame.position); // was: transform.position = frame.position;

        if (characterAnimator != null)
        {
            float direction = frame.facingRight ? 1f : -1f;
            characterAnimator.UpdateFromMovement(direction, true);
        }

        // Once we've caught up to (or passed) the last frame's timestamp,
        // the recording is finished.
        if (currentFrameIndex >= frames.Count - 1 && playbackTimer > frame.timestamp)
        {
            currentFrameIndex = frames.Count; // triggers despawn next tick
        }
    }
}