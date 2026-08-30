using UnityEngine;

// A physical tail built from a small chain of lagging joints (body -> mid
// -> tip), each easing toward the one before it with its own delay. The
// curve is drawn through all joints using Catmull-Rom interpolation, giving
// a natural wave/whip rather than a single fixed-shape bow.
//
// Attach to the Player/Echo root object (or any object on it).

[RequireComponent(typeof(LineRenderer))]
public class SimpleTail : MonoBehaviour
{
    [Header("Setup")]
    public Transform bodyAnchor;        // point on the body the tail attaches from (defaults to self if left empty)

    [Header("Feel — Mid Joint")]
    public float midFollowDelay = 0.08f;
    public float midRestingOffset = 0.2f;

    [Header("Feel — Tip Joint")]
    public float tipFollowDelay = 0.15f;
    public float tipRestingOffset = 0.4f; // total resting length is roughly midRestingOffset + tipRestingOffset

    [Header("Line Appearance")]
    public float startWidth = 0.08f;
    public float endWidth = 0.02f;      // tapers to a point at the tip

    [Header("Curve")]
    public int curveResolution = 12;    // more points = smoother curve

    private LineRenderer line;
    private Vector3 midPosition, midVelocity;
    private Vector3 tipPosition, tipVelocity;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.startWidth = startWidth;
        line.endWidth = endWidth;

        if (bodyAnchor == null) bodyAnchor = transform;

        midPosition = bodyAnchor.position - new Vector3(midRestingOffset, 0, 0);
        tipPosition = midPosition - new Vector3(tipRestingOffset, 0, 0);

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.orange, 0f),
                new GradientColorKey(Color.white, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),   // fully opaque at the body
                new GradientAlphaKey(0f, 1f)    // fully transparent at the tip
            }
);
line.colorGradient = gradient;
    }

    void LateUpdate()
    {
        if (bodyAnchor == null) return;

        line.startWidth = startWidth;
        line.endWidth = endWidth;

        // Each joint eases toward the one before it, with its own delay —
        // this staggered lag is what makes the chain wave naturally instead
        // of moving as one rigid unit.
        midPosition = Vector3.SmoothDamp(midPosition, bodyAnchor.position, ref midVelocity, midFollowDelay);
        tipPosition = Vector3.SmoothDamp(tipPosition, midPosition, ref tipVelocity, tipFollowDelay);

        DrawCurveThrough(bodyAnchor.position, midPosition, tipPosition);
    }

    private void DrawCurveThrough(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // Catmull-Rom needs 4 control points; duplicate the first and last
        // so the curve starts/ends exactly at the body and tip.
        Vector3 c0 = p0;
        Vector3 c1 = p0;
        Vector3 c2 = p1;
        Vector3 c3 = p2;
        Vector3 c4 = p2;

        line.positionCount = curveResolution;
        for (int i = 0; i < curveResolution; i++)
        {
            float t = i / (float)(curveResolution - 1);
            // Sample across two Catmull-Rom segments: c0-c1-c2-c3, then c1-c2-c3-c4
            Vector3 point = t < 0.5f
                ? CatmullRom(c0, c1, c2, c3, t * 2f)
                : CatmullRom(c1, c2, c3, c4, (t - 0.5f) * 2f);
            line.SetPosition(i, point);
        }
    }

    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }
}