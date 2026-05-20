using System;
using static System.Runtime.InteropServices.CallingConvention;
using Fn = System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute;
using Dll = System.Runtime.InteropServices.DllImportAttribute;

namespace Naninovel.StoryEditor
{
    public static class Native
    {
        #if UNITY_EDITOR_OSX
        private const string dll = "Naninovel.StoryEditor.dylib";
        #else
        private const string dll = "Naninovel.StoryEditor";
        #endif

        [Fn(Cdecl)] public delegate void DirtyCb ();
        [Fn(Cdecl)] public delegate IntPtr ReadFile (string name);
        [Fn(Cdecl)] public delegate void WriteFile (string name, string content);
        [Fn(Cdecl)] public delegate IntPtr ListFiles ();
        [Fn(Cdecl)] public delegate void Reload (string text);
        [Fn(Cdecl)] public delegate byte NavCb (string url);
        [Fn(Cdecl)] public delegate void FocusCb (byte focused);
        [Fn(Cdecl)] public delegate void InitCb ();

        [Dll(dll, EntryPoint = "init", CallingConvention = Cdecl)]
        public static extern void Init (
            string fsUserDir, string fsProjDir, string fsProjFilters, DirtyCb fsDirtyCb,
            ReadFile brRead, WriteFile brWrite, ListFiles brList, Reload brReload,
            IntPtr viewHostWindow, string viewHomeUrl, string viewDataDir,
            NavCb viewNavCb, FocusCb viewFocusCb, InitCb viewInitCb);
        [Dll(dll, EntryPoint = "deinit", CallingConvention = Cdecl)]
        public static extern void Deinit ();
        [Dll(dll, EntryPoint = "find_window", CallingConvention = Cdecl)]
        public static extern IntPtr FindWindow (string title);
        [Dll(dll, EntryPoint = "handle_bridging_changes", CallingConvention = Cdecl)]
        public static extern void HandleBridgingChanges (string name);
        [Dll(dll, EntryPoint = "resize_view", CallingConvention = Cdecl)]
        public static extern void ResizeView (double x, double y, double width, double height);
        [Dll(dll, EntryPoint = "set_view_visible", CallingConvention = Cdecl)]
        public static extern void SetViewVisible (byte visible);
        [Dll(dll, EntryPoint = "preview_file", CallingConvention = Cdecl)]
        public static extern void PreviewFile (string uri);
    }
}
