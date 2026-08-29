using UnityEngine;
using System.Collections;

// NEW — not part of the original stub set, but nothing implemented the
// squash/stretch/eyebrow/blink logic that PlayerController and EchoPlayer
// both need to call identically. Lives in Core since it's shared, not
// Player-only. Attach to a "Visual" child on both the Player and Echo
// prefabs, and assign it to characterAnimator in the Inspector on each.

public class CharacterAnimator : MonoBehaviour
{
    [Header("References")]
    public Transform bodyTransform;
    public Transform leftEyebrow;
    public Transform rightEyebrow;
    public Transform leftEye;
    public Transform rightEye;

    [Header("Squash & Stretch")]
    public float stretchAmount = 0.25f;
    public float squashSpeed = 8f;

    [Header("Eyebrows")]
    public float maxEyebrowTilt = 20f; // degrees

    [Header("Blink")]
    public float blinkIntervalMin = 2f;
    public float blinkIntervalMax = 5f;
    public float blinkDuration = 0.12f;

    [Header("Eyes & Eyebrows Movement Offset")]
    public float eyeMoveOffset = 0.05f;      // how far eyes shift toward movement direction
    public float eyebrowMoveOffset = 0.08f;  // how far eyebrows shift toward movement direction
    public float moveOffsetSpeed = 10f;      // how quickly they slide to the offset position

    private Vector3 baseScale;
    private float leftEyeBaseY = 1f;
    private float rightEyeBaseY = 1f;
    private Vector3 leftEyeBasePos;
    private Vector3 rightEyeBasePos;
    private Vector3 leftEyebrowBasePos;
    private Vector3 rightEyebrowBasePos;

    void Awake()
    {
        if (bodyTransform == null) bodyTransform = transform;
        baseScale = bodyTransform.localScale;

        // Cache the artist's original eye scale instead of assuming 1.0 —
        // otherwise "open" state forces eyes to full scale regardless of
        // how they were actually designed.
        if (leftEye != null) leftEyeBaseY = leftEye.localScale.y;
        if (rightEye != null) rightEyeBaseY = rightEye.localScale.y;

        // Cache original local positions so movement offset is always
        // relative to where the artist actually placed each feature.
        if (leftEye != null) leftEyeBasePos = leftEye.localPosition;
        if (rightEye != null) rightEyeBasePos = rightEye.localPosition;
        if (leftEyebrow != null) leftEyebrowBasePos = leftEyebrow.localPosition;
        if (rightEyebrow != null) rightEyebrowBasePos = rightEyebrow.localPosition;

        StartCoroutine(BlinkLoop());
    }

    // direction: roughly -1..1. PlayerController passes raw horizontal
    // input; EchoPlayer passes +1/-1 derived from EchoFrame.facingRight.
    public void UpdateFromMovement(float direction, bool grounded)
    {
        float dir = Mathf.Clamp(direction, -1f, 1f);
        float speedFactor = Mathf.Clamp01(Mathf.Abs(direction));

        float targetX = baseScale.x * (1f + stretchAmount * speedFactor);
        float targetY = baseScale.y * (1f - stretchAmount * speedFactor * 0.5f);
        Vector3 target = new Vector3(targetX, targetY, baseScale.z);
        bodyTransform.localScale = Vector3.Lerp(bodyTransform.localScale, target, Time.deltaTime * squashSpeed);

        float tilt = dir * maxEyebrowTilt;
        if (leftEyebrow != null) leftEyebrow.localRotation = Quaternion.Euler(0, 0, tilt);
        if (rightEyebrow != null) rightEyebrow.localRotation = Quaternion.Euler(0, 0, tilt);

        // Eyes and eyebrows also slide slightly toward the movement
        // direction, on top of the eyebrow tilt — both lerp back to their
        // original position when idle (dir == 0).
        Vector3 eyeTarget = new Vector3(dir * eyeMoveOffset, 0f, 0f);
        Vector3 eyebrowTarget = new Vector3(dir * eyebrowMoveOffset, 0f, 0f);

        if (leftEye != null)
            leftEye.localPosition = Vector3.Lerp(leftEye.localPosition, leftEyeBasePos + eyeTarget, Time.deltaTime * moveOffsetSpeed);
        if (rightEye != null)
            rightEye.localPosition = Vector3.Lerp(rightEye.localPosition, rightEyeBasePos + eyeTarget, Time.deltaTime * moveOffsetSpeed);
        if (leftEyebrow != null)
            leftEyebrow.localPosition = Vector3.Lerp(leftEyebrow.localPosition, leftEyebrowBasePos + eyebrowTarget, Time.deltaTime * moveOffsetSpeed);
        if (rightEyebrow != null)
            rightEyebrow.localPosition = Vector3.Lerp(rightEyebrow.localPosition, rightEyebrowBasePos + eyebrowTarget, Time.deltaTime * moveOffsetSpeed);
    }

    IEnumerator BlinkLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(blinkIntervalMin, blinkIntervalMax));
            SetEyeScale(0.1f);
            yield return new WaitForSeconds(blinkDuration);
            SetEyeScale(1f); // 1f here means "fully open", scaled below
        }
    }

    // yScaleFraction: 1f = fully open (original artist scale), smaller = more closed.
    void SetEyeScale(float yScaleFraction)
    {
        if (leftEye != null) leftEye.localScale = new Vector3(leftEye.localScale.x, leftEyeBaseY * yScaleFraction, leftEye.localScale.z);
        if (rightEye != null) rightEye.localScale = new Vector3(rightEye.localScale.x, rightEyeBaseY * yScaleFraction, rightEye.localScale.z);
    }
}