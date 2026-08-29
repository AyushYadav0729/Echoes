using UnityEngine;

// Minimal stand-in for the real completion screen (Person 3's HUDManager
// will build the polished version). Listens to the same GameEvents.OnLevelCleared
// event, so replacing this later doesn't require touching Player/Goal logic at all.
//
// Attach this to any empty GameObject in the scene (e.g. a "GameManager" object).

public class LevelCompletionUI : MonoBehaviour
{
    private bool levelCleared = false;
    private int attemptCountAtClear = 0;

    void OnEnable()
    {
        GameEvents.OnLevelCleared += HandleLevelCleared;
        GameEvents.OnAttemptEnded += HandleAttemptEnded;
    }

    void OnDisable()
    {
        GameEvents.OnLevelCleared -= HandleLevelCleared;
        GameEvents.OnAttemptEnded -= HandleAttemptEnded;
    }

    private int lastKnownAttemptNumber = 0;

    void HandleAttemptEnded(AttemptResult result)
    {
        lastKnownAttemptNumber = result.attemptNumber;
    }

    void HandleLevelCleared()
    {
        levelCleared = true;
        attemptCountAtClear = lastKnownAttemptNumber + 1; // the clearing attempt itself
        Debug.Log($"LEVEL CLEARED in {attemptCountAtClear} attempts!");
        Time.timeScale = 0f; // simple pause so you can visually confirm the moment it fires
    }

    // Temporary on-screen confirmation using OnGUI so you don't need
    // a Canvas set up yet — Person 3 will replace this with real UI.
    void OnGUI()
    {
        if (!levelCleared) return;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 32;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;

        GUI.Label(new Rect(0, Screen.height / 2 - 50, Screen.width, 100),
            $"LEVEL CLEARED\nCleared in {attemptCountAtClear} attempts", style);
    }
}
