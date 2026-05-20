using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Naninovel.StoryEditor
{
    /// <summary>
    /// Manages the story editor embedded webview.
    /// </summary>
    public static class StoryEditor
    {
        /// <summary>
        /// Whether the editor is currently initialized.
        /// </summary>
        public static bool Initialized { get; private set; }
        /// <summary>
        /// Whether the editor is currently initializing.
        /// </summary>
        public static bool Initializing => initCts != null;

        // private const string homeUrl = "http://localhost:5173/editor/";
        private const string homeUrl = "https://naninovel.com/editor/";

        [CanBeNull] private static CancellationTokenSource initCts;
        [CanBeNull] private static string pendingShowScriptPath;

        /// <summary>
        /// Initializes and embeds the editor webview into the Unity tab.
        /// </summary>
        public static async Task Initialize ()
        {
            if (Initialized || Initializing) return;

            Initialized = false;
            var cts = initCts = new();

            if (await FindWindow(cts.Token) is not { } window) return;
            if (cts.Token.IsCancellationRequested) return;

            var dataDir = PathUtils.Combine(Application.persistentDataPath, "Naninovel");
            var userDir = PathUtils.Combine(Application.persistentDataPath, "Naninovel/User");
            var projDir = Application.dataPath;
            var filters = string.Join('|',
                ToProjectFileUri(PackagePath.EditorDataPath),
                ToProjectFileUri(PackagePath.ScenarioRoot)
            );

            try
            {
                Native.Init(userDir, projDir, filters, HandleAssetsDirty,
                    Bridging.Read, Bridging.Write, Bridging.List, HandleHotReload,
                    window, homeUrl, dataDir, HandleViewNavigation, HandleFocus, HandleViewInitialized);
                Bridging.Initialize();
                Selection.selectionChanged += HandleSelectionChanged;
                Initialized = true;
            }
            finally
            {
                cts.Dispose();
                if (initCts == cts) initCts = null;
            }
        }

        /// <summary>
        /// Deinitializes the editor and terminates the embedded webview thread.
        /// </summary>
        public static void Deinitialize ()
        {
            if (Initialized)
            {
                Bridging.Deinitialize();
                Native.Deinit();
            }
            Initialized = false;
            try { initCts?.Cancel(); }
            catch (ObjectDisposedException) { }
            initCts = null;
            Selection.selectionChanged -= HandleSelectionChanged;
        }

        /// <summary>
        /// Opens scenario script with the specified asset path inside the story editor.
        /// </summary>
        public static void ShowScript (string assetPath)
        {
            if (Initialized) Native.PreviewFile(ToProjectFileUri(assetPath));
            else pendingShowScriptPath = assetPath;
            Window.DockWithInspector();
        }

        private static void HandleViewInitialized ()
        {
            ThreadUtils.InvokeOnUnityThread(() => {
                if (pendingShowScriptPath != null)
                {
                    ShowScript(pendingShowScriptPath);
                    pendingShowScriptPath = null;
                }
            });
        }

        private static void HandleFocus (byte focused)
        {
            ThreadUtils.InvokeOnUnityThread(() => {
                EditorProxy.StoryEditorFocused = focused == 1;
            });
        }

        private static void HandleAssetsDirty ()
        {
            ThreadUtils.InvokeOnUnityThread(AssetDatabase.Refresh);
        }

        private static void HandleHotReload (string text)
        {
            ThreadUtils.InvokeOnUnityThread(() => {
                if (!Engine.Initialized || Engine.Behaviour is not RuntimeBehaviour) return;
                var scripts = Engine.GetServiceOrErr<IScriptManager>();
                var player = Engine.GetServiceOrErr<IScriptPlayer>();
                if (!scripts.Configuration.HotReloadScripts || !player.MainTrack.PlayedScript) return;
                var path = player.MainTrack.PlayedScript.Path;
                scripts.ScriptLoader.Update(path, Compiler.CompileScript(path, text));
                player.Reload().Forget();
            });
        }

        private static byte HandleViewNavigation (string url)
        {
            if (url.StartsWithOrdinal(homeUrl)) return 1;
            ThreadUtils.InvokeOnUnityThread(() => Application.OpenURL(url));
            return 0;
        }

        private static async Task<IntPtr?> FindWindow (CancellationToken token)
        {
            var window = IntPtr.Zero;
            while (!token.IsCancellationRequested && window == IntPtr.Zero)
                if ((window = Native.FindWindow(ResolveTitle())) == IntPtr.Zero)
                    await Task.Delay(100, token);
            return window == IntPtr.Zero ? null : window;

            static string ResolveTitle ()
            {
                // On Windows, HWND title equals C# full type name of the tab (both docked und undocked).
                if (Application.platform == RuntimePlatform.WindowsEditor) return typeof(Window).FullName;
                // On macOS, when undocked, window title equals display title of the tab.
                // When docked, the tab is an NSView without any explicit identifiable traits,
                // so we use the tab's screen-space center position to find the associated NSView.
                var tab = EditorWindow.GetWindow<Window>();
                if (!tab.docked) return tab.titleContent.text;
                return $"{tab.position.center.x}|{tab.position.center.y}";
            }
        }

        private static void HandleSelectionChanged ()
        {
            if (!Initialized) return;
            if (!Configuration.GetOrDefault<ScriptsConfiguration>().ShowSelectedScript) return;
            if (Selection.activeObject is not Script script || Selection.objects.Length > 1) return;
            var assetPath = AssetDatabase.GetAssetPath(script);
            if (!string.IsNullOrEmpty(assetPath))
                Native.PreviewFile(ToProjectFileUri(assetPath));
        }

        private static string ToProjectFileUri (string assetPath)
        {
            return assetPath.GetAfterFirst("Assets");
        }
    }
}
