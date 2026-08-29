using System.Collections.Generic;
using UnityEngine;

// PERSON 1 — fill in the logic inside each method.

public class EchoPlayer : MonoBehaviour
{
    private List<EchoFrame> frames;
    private int currentFrameIndex = 0;

    public void Init(List<EchoFrame> recording)
    {
        frames = recording;
        currentFrameIndex = 0;
        // TODO: reset position to level start (frames[0].position, or a shared spawn point)
    }

    void FixedUpdate()
    {
        if (frames == null || currentFrameIndex >= frames.Count)
        {
            // TODO: recording finished — despawn this echo (Destroy(gameObject) or disable it)
            return;
        }

        // TODO: apply frames[currentFrameIndex] to transform.position and facing direction
        // TODO: trigger the same squash/stretch + eyebrow animation logic the player uses
        // (reuse the same animation code/component — don't duplicate it)

        currentFrameIndex++;
    }
}
