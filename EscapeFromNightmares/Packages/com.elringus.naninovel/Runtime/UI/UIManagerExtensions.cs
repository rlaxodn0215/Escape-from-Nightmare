using System;
using System.Collections.Generic;
using System.Linq;
using Naninovel.UI;
using UnityEngine.UI;

namespace Naninovel
{
    /// <summary>
    /// Provides extension methods for <see cref="IUIManager"/>.
    /// </summary>
    public static class UIManagerExtensions
    {
        /// <summary>
        /// Attempts to retrieve managed UI of the specified type <typeparamref name="T"/>.
        /// </summary>
        public static bool TryGetUI<T> (this IUIManager manager, out T ui) where T : class, IManagedUI
        {
            return (ui = manager?.GetUI<T>()) != null;
        }

        /// <summary>
        /// Attempts to retrieve managed UI with the specified name.
        /// </summary>
        public static bool TryGetUI (this IUIManager manager, string name, out IManagedUI ui)
        {
            return (ui = manager?.GetUI(name)) != null;
        }

        /// <summary>
        /// Returns managed UI of the specified UI resource name or throws when not available.
        /// </summary>
        public static IManagedUI GetUIOrErr (this IUIManager manager, string name)
        {
            return manager?.GetUI(name) ?? throw new Error($"UI with '{name}' name is not available.");
        }

        /// <summary>
        /// Returns managed UI of the specified type <typeparamref name="T"/> or throws when not available.
        /// </summary>
        public static T GetUIOrErr<T> (this IUIManager manager) where T : class, IManagedUI
        {
            return manager?.GetUI<T>() ?? throw new Error($"UI of '{typeof(T)}' type is not available.");
        }

        /// <summary>
        /// Rents a pooled list and collects all the managed UI instances.
        /// </summary>
        public static IDisposable RentUIs (this IUIManager manager, out List<IManagedUI> uis)
        {
            var rent = ListPool<IManagedUI>.Rent(out uis);
            manager.CollectUIs(uis);
            return rent;
        }

        /// <summary>
        /// Attempts to select game object under the topmost visible managed UI.
        /// Returns true when object was found and focused; false otherwise.
        /// </summary>
        public static bool FocusTop (this IUIManager manager)
        {
            using var _ = manager.RentUIs(out var uis);
            var top = uis.OfType<CustomUI>()
                .Where(ui => ui.Visible)
                .OrderByDescending(ui => ui.SortingOrder).FirstOrDefault();
            if (!top) return false;
            if (top.FocusObject && top.FocusObject.activeInHierarchy)
                EventUtils.Select(top.FocusObject);
            var selectable = top.Selectables
                .FirstOrDefault(s => s.Navigation.mode != Navigation.Mode.None &&
                                     s.Selectable.gameObject.activeInHierarchy).Selectable;
            if (!selectable) return false;
            EventUtils.Select(selectable.gameObject);
            return true;
        }
    }
}
