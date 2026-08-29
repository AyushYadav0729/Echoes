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

    public void Init(List<EchoFrame> recording)
    {
        frames = recording;
        currentFrameIndex = 0;

        if (frames != null && frames.Count > 0)
            transform.position = frames[0].position;

        if (trail != null)
            trail.Clear();
    }

    void FixedUpdate()
    {
        if (frames == null || currentFrameIndex >= frames.Count)
        {
            // Recording finished — echo simply stops existing, no special state.
            Destroy(gameObject);
            return;
        }

        EchoFrame frame = frames[currentFrameIndex];
        transform.position = frame.position;

        if (characterAnimator != null)
        {
            float direction = frame.facingRight ? 1f : -1f;
            characterAnimator.UpdateFromMovement(direction, true);
        }

        currentFrameIndex++;
    }
}