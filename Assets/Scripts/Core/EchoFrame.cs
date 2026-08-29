using UnityEngine;

// The recorded data format for a single frame of a player's run.
// This shape is shared across Player, Echo, and AI narration code —
// do not change it after Person 1 starts recording without telling everyone.

[System.Serializable]
public struct EchoFrame
{
    public float timestamp;
    public Vector2 position;
    public bool facingRight;
    public bool isPressingPlate; // optional, useful for debugging/AI narration
}
