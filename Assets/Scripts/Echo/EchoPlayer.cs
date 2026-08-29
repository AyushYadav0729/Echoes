using System.Collections.Generic;
using UnityEngine;

// PERSON 1 — filled in. Class/method names and signatures unchanged.

public class EchoPlayer : MonoBehaviour
{
    [Header("References — assign in Inspector")]
    public CharacterAnimator characterAnimator; // same shared component the player uses
    public TrailRenderer trail;                  // onion-skin trail — echoes only

    private List<EchoFrame> frames;
    private int currentFrameIndex;
    private float playbackTimer;

    public void Init(List<EchoFrame> recording)
    {
        frames = recording;
        currentFrameIndex = 0;
        playbackTimer = 0f;

        if (frames != null && frames.Count > 0)
            transform.position = frames[0].position;

        if (trail != null)
            trail.Clear();
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
        transform.position = frame.position;

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