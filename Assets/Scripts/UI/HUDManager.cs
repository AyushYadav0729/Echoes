using UnityEngine;
using UnityEngine.UI;
using TMPro; // if using TextMeshPro — swap to regular Text if not

// PERSON 3 — fill in the logic inside each method.
// Listens to GameEvents (from Core/) instead of needing direct references
// to Player/Echo scripts — keeps UI decoupled from gameplay code.

public class HUDManager : MonoBehaviour
{
    [Header("UI References — assign in Inspector")]
    public TMP_Text attemptCounterText;
    public TMP_Text echoCountText;
    public GameObject completionScreen;
    public TMP_Text completionAttemptsText;

    private int attemptNumber = 1;

    void OnEnable()
    {
        GameEvents.OnAttemptEnded += HandleAttemptEnded;
        GameEvents.OnEchoCountChanged += HandleEchoCountChanged;
        GameEvents.OnLevelCleared += HandleLevelCleared;
    }

    void OnDisable()
    {
        GameEvents.OnAttemptEnded -= HandleAttemptEnded;
        GameEvents.OnEchoCountChanged -= HandleEchoCountChanged;
        GameEvents.OnLevelCleared -= HandleLevelCleared;
    }

    void HandleAttemptEnded(AttemptResult result)
    {
        // TODO: increment attemptNumber, update attemptCounterText
    }

    void HandleEchoCountChanged(int count)
    {
        // TODO: update echoCountText (e.g. "Echoes: 2/3")
    }

    void HandleLevelCleared()
    {
        // TODO: show completionScreen, set completionAttemptsText to "Cleared in X attempts"
    }
}
