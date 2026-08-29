using UnityEngine;

// Lets plates/switches react identically whether triggered
// by the live player or an echo, without needing to know which.

public interface IInteractable
{
    void OnPressed(GameObject presser);
    void OnReleased(GameObject presser);
}
