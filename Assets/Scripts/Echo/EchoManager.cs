using System.Collections.Generic;
using UnityEngine;

// NEW — not part of the original stub set, but "echo cap/rotation logic"
// is explicitly Person 1's job per the design doc, and EchoPlayer.Init()
// needs something to call it. Raises GameEvents.OnEchoCountChanged so
// HUDManager stays decoupled from this script (matches the pattern
// already used by AttemptResult/OnAttemptEnded).

public class EchoManager : MonoBehaviour
{
    public GameObject echoPrefab;
    public Transform levelStartPoint;
    public int maxEchoes = 3;
    [Range(0f, 1f)] public float bodyAlpha = 0.45f;
    [Range(0f, 1f)] public float faceAlpha = 0.85f; // eyes/eyebrows stay more opaque so they don't wash out

    private static readonly Color[] EchoTints =
    {
        new Color(0.6f, 0.8f, 1f), // pale blue
        new Color(0.8f, 0.6f, 1f), // pale purple
        new Color(0.6f, 1f, 0.7f)  // pale green
    };

    private readonly Queue<List<EchoFrame>> echoHistory = new Queue<List<EchoFrame>>(); // oldest first
    private readonly List<GameObject> activeEchoObjects = new List<GameObject>();

    public void AddEcho(List<EchoFrame> frames)
    {
        if (frames == null || frames.Count == 0) return;

        if (echoHistory.Count >= maxEchoes)
            echoHistory.Dequeue(); // oldest replaced

        echoHistory.Enqueue(frames);
        SpawnAllEchoesFromLevelStart();
        GameEvents.RaiseEchoCountChanged(echoHistory.Count);
    }

    // Rebuilds every active echo so all echoes start their replay from
    // level start, in sync with the player's respawn.
    public void SpawnAllEchoesFromLevelStart()
    {
        foreach (var obj in activeEchoObjects)
            if (obj != null) Destroy(obj);
        activeEchoObjects.Clear();

        int i = 0;
        foreach (var frames in echoHistory)
        {
            GameObject echoObj = Instantiate(echoPrefab, levelStartPoint.position, Quaternion.identity);
            echoObj.GetComponent<EchoPlayer>().Init(frames);
            ApplyTint(echoObj, EchoTints[i % EchoTints.Length]);
            activeEchoObjects.Add(echoObj);
            i++;
        }
    }

    // RESTART button: wipe everything immediately.
    public void ClearAllEchoes()
    {
        echoHistory.Clear();
        foreach (var obj in activeEchoObjects)
            if (obj != null) Destroy(obj);
        activeEchoObjects.Clear();
        GameEvents.RaiseEchoCountChanged(0);
    }

    // Call when loading a new level — echo pool always resets to empty.
    public void ResetForNewLevel() => ClearAllEchoes();

    public int ActiveEchoCount => echoHistory.Count;

    void ApplyTint(GameObject echoObj, Color tint)
    {
        Color c = tint;
        c.a = bodyAlpha;

        var animator = echoObj.GetComponentInChildren<CharacterAnimator>();
        if (animator != null)
        {
            var bodyRenderer = animator.bodyTransform != null ? animator.bodyTransform.GetComponent<SpriteRenderer>() : null;
            if (bodyRenderer != null) bodyRenderer.color = c;

            // Eyes/eyebrows are solid black sprites — multiplying by any
            // tint still gives black, so only alpha is adjustable. Kept
            // more opaque than the body so the face stays readable.
            FadeAlpha(animator.leftEye, faceAlpha);
            FadeAlpha(animator.rightEye, faceAlpha);
            FadeAlpha(animator.leftEyebrow, faceAlpha);
            FadeAlpha(animator.rightEyebrow, faceAlpha);
        }
        else
        {
            foreach (var r in echoObj.GetComponentsInChildren<SpriteRenderer>())
                r.color = c;
        }

        var trail = echoObj.GetComponentInChildren<TrailRenderer>();
        if (trail != null)
        {
            trail.startColor = c;
            trail.endColor = new Color(c.r, c.g, c.b, 0f); // fades to transparent
        }
    }

    void FadeAlpha(Transform t, float alpha)
    {
        if (t == null) return;
        var r = t.GetComponent<SpriteRenderer>();
        if (r == null) return;
        Color col = r.color;
        col.a = alpha;
        r.color = col;
    }
}