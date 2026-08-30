using System;

// A tiny static event hub so systems can talk to each other without
// direct references — e.g. UI can listen for "attempt ended" without
// the Player script needing to know the UI exists.
//
// Usage:
//   Raising:    GameEvents.RaiseAttemptEnded(result);
//   Listening:  GameEvents.OnAttemptEnded += MyHandlerMethod;
//   (remember to unsubscribe in OnDisable to avoid memory leaks)

public static class GameEvents
{
    public static event Action OnLevelReset;
    public static void RaiseLevelReset() => OnLevelReset?.Invoke();
    public static event Action<AttemptResult> OnAttemptEnded;
    public static event Action<int> OnEchoCountChanged;
    public static event Action OnLevelCleared;

    public static void RaiseAttemptEnded(AttemptResult result) => OnAttemptEnded?.Invoke(result);
    public static void RaiseEchoCountChanged(int count) => OnEchoCountChanged?.Invoke(count);
    public static void RaiseLevelCleared() => OnLevelCleared?.Invoke();
}
