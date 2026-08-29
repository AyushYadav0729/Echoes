using UnityEngine;
using UnityEngine.UI;
using TMPro; // if using TextMeshPro — swap to regular Text if not

// Filled in. Class/method names and signatures unchanged from the template.
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

    void Start()
    {
        if (attemptCounterText != null)
            attemptCounterText.text = $"Attempt {attemptNumber}";
        if (echoCountText != null)
            echoCountText.text = "Echoes: 0/3";
        if (completionScreen != null)
            completionScreen.SetActive(false);
    }

    void HandleAttemptEnded(AttemptResult result)
    {
        attemptNumber = result.attemptNumber + 1; // the attempt that's about to start
        if (attemptCounterText != null)
            attemptCounterText.text = $"Attempt {attemptNumber}";
    }

    void HandleEchoCountChanged(int count)
    {
        if (echoCountText != null)
            echoCountText.text = $"Echoes: {count}/3";
    }

    void HandleLevelCleared()
    {
        if (completionScreen != null)
            completionScreen.SetActive(true);
        if (completionAttemptsText != null)
            completionAttemptsText.text = $"Cleared in {attemptNumber} attempts";
    }
}