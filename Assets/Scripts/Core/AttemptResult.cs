using UnityEngine;

// Summarizes how an attempt ended. Fed to UI (attempt counter) and
// to the AI narration system, so those systems don't need direct
// references to the Player/Echo scripts.

public enum AttemptEndReason { Died, EchoButton, LevelCleared }

[System.Serializable]
public struct AttemptResult
{
    public AttemptEndReason reason;
    public float durationSeconds;
    public Vector2 endPosition;
    public int attemptNumber;
}
