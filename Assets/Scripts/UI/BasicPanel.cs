// -----------------------------------------------------------------------------
// Codex comment pass: Basic Panel
// Role: Updates visible Unity UI elements so the screen reflects the current game, save, inventory, or title state.
// Scope: This script belongs to UI\BasicPanel.cs and keeps its behavior isolated to that folder's responsibility.
// Maintenance note: These comments explain intent only; they do not change serialized fields, scene wiring, or runtime behavior.
// -----------------------------------------------------------------------------

using UnityEngine;

namespace EscapeFromNightmare
{
    // Presentation controller for Basic Panel UI elements, keeping references cached and visuals synchronized.
    public class BasicPanel : MonoBehaviour
    {
        // Makes the related panel or visual element visible and fills in its current content.
        public void Show()
        {
            gameObject.SetActive(true);
        }

        // Hides the related panel or visual element and clears transient interaction state.
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
