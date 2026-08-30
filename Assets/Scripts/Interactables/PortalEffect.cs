using UnityEngine;

// Simple portal effect: scales up, waits, scales down, destroys itself.
// No coroutines — just a plain Update() loop with a state flag.

public class PortalEffect : MonoBehaviour
{
    [Header("Timing")]
    public float expandTime = 0.15f;
    public float holdTime = 0.1f;
    public float shrinkTime = 0.15f;
    public float fullScale = 1f;

    private enum State { Expanding, Holding, Shrinking, Done }
    private State state = State.Expanding;
    private float timer = 0f;

    void Awake()
    {
        transform.localScale = Vector3.zero;
    }

    void Update()
    {
        timer += Time.deltaTime;

        switch (state)
        {
            case State.Expanding:
                float expandT = Mathf.Clamp01(timer / expandTime);
                transform.localScale = Vector3.one * (fullScale * expandT);
                if (expandT >= 1f) { state = State.Holding; timer = 0f; }
                break;

            case State.Holding:
                if (timer >= holdTime) { state = State.Shrinking; timer = 0f; }
                break;

            case State.Shrinking:
                float shrinkT = Mathf.Clamp01(timer / shrinkTime);
                transform.localScale = Vector3.one * (fullScale * (1f - shrinkT));
                if (shrinkT >= 1f) { state = State.Done; }
                break;

            case State.Done:
                Destroy(gameObject);
                break;
        }
    }
}