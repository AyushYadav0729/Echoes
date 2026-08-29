using System.Collections.Generic;
using UnityEngine;

// PERSON 1 — filled in. Class/method names and signatures unchanged.

public class EchoRecorder : MonoBehaviour
{
    // Should match Time.fixedDeltaTime (default 0.02s) so EchoPlayer's
    // one-index-per-FixedUpdate playback stays in sync. If you change
    // Project Settings > Time > Fixed Timestep, update this to match.
    public float sampleInterval = 0.02f;

    private List<EchoFrame> frames = new List<EchoFrame>();
    private bool isRecording = true;
    private float sampleTimer;
    private float attemptStartTime;
    private PlayerController playerController;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        attemptStartTime = Time.fixedTime;
    }

    void FixedUpdate()
    {
        if (!isRecording) return;

        sampleTimer += Time.fixedDeltaTime;
        if (sampleTimer < sampleInterval) return;
        sampleTimer = 0f;

        frames.Add(new EchoFrame
        {
            timestamp = Time.fixedTime - attemptStartTime,
            position = transform.position,
            facingRight = playerController == null || playerController.FacingRight,
            isPressingPlate = playerController != null && playerController.IsOnInteractable
        });
    }

    public List<EchoFrame> StopAndGetRecording()
    {
        isRecording = false;
        return new List<EchoFrame>(frames); // copy so the original can be cleared/reused
    }

    public void ResetRecording()
    {
        frames.Clear();
        sampleTimer = 0f;
        attemptStartTime = Time.fixedTime;
        isRecording = true;
    }
}