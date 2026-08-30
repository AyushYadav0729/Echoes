using UnityEngine;

public class LevelCompletionUI : MonoBehaviour
{
    private bool levelCleared = false;
    private int lastKnownAttemptNumber = 0;
    private int attemptCountAtClear = 0;

    void OnEnable()
    {
        Debug.Log("LevelCompletionUI ENABLED and listening for events.");

        GameEvents.OnLevelCleared += HandleLevelCleared;
        GameEvents.OnAttemptEnded += HandleAttemptEnded;
    }

    void OnDisable()
    {
        GameEvents.OnLevelCleared -= HandleLevelCleared;
        GameEvents.OnAttemptEnded -= HandleAttemptEnded;
    }

    void HandleAttemptEnded(AttemptResult result)
    {
        lastKnownAttemptNumber = result.attemptNumber;

        Debug.Log("Attempt ended: " + lastKnownAttemptNumber);
    }

    void HandleLevelCleared()
    {
        Debug.Log("LEVEL CLEARED EVENT RECEIVED!");

        if (levelCleared)
            return;

        levelCleared = true;

        attemptCountAtClear = lastKnownAttemptNumber + 1;

        Debug.Log($"LEVEL CLEARED in {attemptCountAtClear} attempts!");

        Time.timeScale = 0f;
    }

    void OnGUI()
    {
        if (!levelCleared)
            return;

        GUIStyle style = new GUIStyle(GUI.skin.label);

        style.fontSize = 40;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;

        GUI.Label(
            new Rect(
                0,
                Screen.height / 2 - 50,
                Screen.width,
                100
            ),
            $"LEVEL CLEARED\nCleared in {attemptCountAtClear} attempts",
            style
        );
    }
}