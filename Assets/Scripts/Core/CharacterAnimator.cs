using UnityEngine;
using System.Collections;

// Attach to a "Visual" child on both Player and Echo prefabs.
//
// HIERARCHY REQUIRED for the lean to look like bending-from-a-base
// (water in a glass) instead of the whole object spinning:
//
//   Player (root)
//     └── LeanPivot (empty, positioned at the BOTTOM of the sprite)
//           └── SpriteVisual (the actual sprite, offset upward so its
//                              bottom edge sits at LeanPivot's origin)
//
// Assign leanPivot = LeanPivot, bodyTransform = SpriteVisual.
// Squash/stretch scales the sprite; the lean rotates the pivot, so the
// base stays planted and only the top sways — that's what reads as
// "bending" rather than "rotating."

public class CharacterAnimator : MonoBehaviour
{
    [Header("References")]
    public Transform bodyTransform;   // the sprite itself — gets scaled (squash/stretch)
    public Transform leanPivot;       // empty at the base — gets rotated (the "bend")
    public Transform leftEyebrow;
    public Transform rightEyebrow;
    public Transform leftEye;
    public Transform rightEye;

    [Header("Squash & Stretch")]
    public float stretchAmount = 0.25f;
    public float squashSpeed = 15f;

    [Header("Water Bend (lean from base on accel/decel)")]
    public float leanAmount = 18f;     // max degrees the top can lean
    public float leanSpeed = 10f;
    public float leanDamping = 6f;

    [Header("Eyebrows")]
    public float maxEyebrowTilt = 20f; // degrees

    [Header("Blink")]
    public float blinkIntervalMin = 2f;
    public float blinkIntervalMax = 5f;
    public float blinkDuration = 0.12f;

    private Vector3 baseScale;
    private float lastDirection = 0f;
    private float leanVelocity = 0f;
    private float currentLean = 0f;

    void Awake()
    {
        if (bodyTransform == null) bodyTransform = transform;
        if (leanPivot == null) leanPivot = transform; // falls back to rotating self if no pivot set
        baseScale = bodyTransform.localScale;
        StartCoroutine(BlinkLoop());
    }

    // direction: roughly -1..1. PlayerController passes raw horizontal
    // input; EchoPlayer passes +1/-1 derived from EchoFrame.facingRight.
    public void UpdateFromMovement(float direction, bool grounded)
    {
        float dir = Mathf.Clamp(direction, -1f, 1f);
        float speedFactor = Mathf.Clamp01(Mathf.Abs(direction));

        // --- Detect acceleration (change in direction/speed) — this is
        // what makes it lean on START/STOP of movement, like water
        // reacting to force rather than just leaning while moving. ---
        float accel = dir - lastDirection;
        lastDirection = dir;

        // Spring-damper toward a lean target based on acceleration —
        // gives the overshoot/settle wobble instead of a clean snap.
        float leanTarget = -accel * leanAmount * 5f; // lean opposite the direction of acceleration
        float springForce = (leanTarget - currentLean) * leanSpeed;
        leanVelocity += springForce * Time.deltaTime;
        leanVelocity *= 1f - Mathf.Clamp01(leanDamping * Time.deltaTime); // damping so it settles
        currentLean += leanVelocity * Time.deltaTime;

        // Apply the lean as a rotation on the PIVOT (base-anchored) —
        // this is what makes it bend rather than spin.
        leanPivot.localRotation = Quaternion.Euler(0, 0, currentLean);

        // --- Squash/stretch on the sprite itself ---
        float targetX = baseScale.x * (1f + stretchAmount * speedFactor);
        float targetY = baseScale.y * (1f - stretchAmount * speedFactor * 0.5f);
        Vector3 target = new Vector3(targetX, targetY, baseScale.z);

        // Snap fast into the stretch, ease back slower — reads as punchier
        // than a single constant lerp speed in both directions.
        float currentSpeed = speedFactor > 0f ? squashSpeed * 2f : squashSpeed * 0.6f;
        bodyTransform.localScale = Vector3.Lerp(bodyTransform.localScale, target, Time.deltaTime * currentSpeed);

        // --- Eyebrows still follow direction as before ---
        float tilt = dir * maxEyebrowTilt;
        if (leftEyebrow != null) leftEyebrow.localRotation = Quaternion.Euler(0, 0, tilt);
        if (rightEyebrow != null) rightEyebrow.localRotation = Quaternion.Euler(0, 0, tilt);
    }

    IEnumerator BlinkLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(blinkIntervalMin, blinkIntervalMax));
            SetEyeScale(0.1f);
            yield return new WaitForSeconds(blinkDuration);
            SetEyeScale(1f);
        }
    }

    void SetEyeScale(float yScale)
    {
        if (leftEye != null) leftEye.localScale = new Vector3(leftEye.localScale.x, yScale, leftEye.localScale.z);
        if (rightEye != null) rightEye.localScale = new Vector3(rightEye.localScale.x, yScale, rightEye.localScale.z);
    }
}