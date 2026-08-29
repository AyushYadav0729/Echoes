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

    private Vector3 baseScale;

    void Awake()
    {
        if (bodyTransform == null) bodyTransform = transform;
        baseScale = bodyTransform.localScale;
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