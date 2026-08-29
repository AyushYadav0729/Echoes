using System.Collections.Generic;
using UnityEngine;

// PERSON 1 — fill in the logic inside each method.
// Do not rename the class, methods, or change their signatures.

public class EchoRecorder : MonoBehaviour
{
    private List<EchoFrame> frames = new List<EchoFrame>();
    private bool isRecording = true;

    void FixedUpdate()
    {
        if (!isRecording) return;

        // TODO: build an EchoFrame from current transform.position,
        // facing direction, and Time.fixedTime, then frames.Add(it)
    }

    public List<EchoFrame> StopAndGetRecording()
    {
        isRecording = false;
        // TODO: return a copy of frames (so the original list can be cleared/reused if needed)
        return frames;
    }

    public void ResetRecording()
    {
        frames.Clear();
        isRecording = true;
    }
}
