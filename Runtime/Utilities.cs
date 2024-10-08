using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace UnityTest
{
    public static class Utilities
    {
        public const string debugTag = "[UnityTest]";

        public const string editorPrefs = "UnityTest";
        public const string guidPrefs = "UnityTest/GUIDs";
        public const char guidDelimiter = '\n';
        public const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static;

        /// <summary>
        /// Location of the "Assets" folder.
        /// </summary>
        public static string assetsPath { get; } = Path.GetFullPath(Application.dataPath);
        /// <summary>
        /// Location of the Unity project.
        /// </summary>
        public static string projectPath { get; } = Path.GetFullPath(Path.GetDirectoryName(assetsPath));
        /// <summary>
        /// Location of the "Packages" folder.
        /// </summary>
        public static string packagesPath { get; } = Path.GetFullPath(Path.Join(projectPath, "Packages"));
        /// <summary>
        /// Location of the "Packages/UnityTest/Runtime" folder.
        /// </summary>
        public static string runtimeDir { get; } = Path.GetFullPath(Path.Join(packagesPath, "UnityTest", "Runtime"));
        /// <summary>
        /// Location of the "Packages/UnityTest/Runtime/Data" folder.
        /// </summary>
        public static string dataPath { get; } = Path.GetFullPath(Path.Join(runtimeDir, "Data"));

        /// <summary>
        /// The file located at "Packages/UnityTest/Runtime/ExampleTests.cs"
        /// </summary>
        public static string exampleTestsFile { get; } = Path.GetFullPath(Path.Join(runtimeDir, "ExampleTests.cs"));

        /// <summary>
        /// True if the editor is using the theme called "DarkSkin". Otherwise, false.
        /// </summary>
        public static bool isDarkTheme = true;

        /// <summary>
        /// HTML color green. Adapts to DarkSkin and LightSkin.
        /// </summary>
        public static string green { get { if (isDarkTheme) return "#50C878"; return "#164f00"; } }
        /// <summary>
        /// HTML color red. Adapts to DarkSkin and LightSkin.
        /// </summary>
        public static string red { get { if (isDarkTheme) return "red"; return "red"; } }

        public static float searchBarMinWidth = 80f;
        public static float searchBarMaxWidth = 300f;

        public static bool IsSceneEmpty()
        {
            GameObject[] objects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            if (objects == null) return true;
            if (objects.Length == 0) return true;
            if (objects.Length != 2) return false;
            return objects[0].name == "MainCamera" && objects[1].name == "Directional Light";
        }

        /// <summary>
        /// Return each successive directory, starting at the directory of the given path and moving outward toward the root path, 
        /// until the root directory is reached.
        /// </summary>
        /// <param name="path">The path to iterate over</param>
        /// <param name="reverse">If true, the iterator starts at the root path and works out to the directory of the given path.</param>
        public static IEnumerable<string> IterateDirectories(string path, bool reverse = false)
        {
            if (reverse)
            {
                List<string> enumerator = new List<string>(IterateDirectories(path, false));
                enumerator.Reverse();
                foreach (string item in enumerator)
                {
                    yield return item;
                }
            }
            else
            {
                string directory = Path.GetDirectoryName(path);
                while (!string.IsNullOrEmpty(directory))
                {
                    yield return directory;
                    directory = Path.GetDirectoryName(directory);
                }
            }
        }


        /// <summary>
        /// For a given full file path, return a new path that starts either with "Assets" or "Packages", in the way that
        /// Unity expects for function AssetDatabase.LoadAssetAtPath().
        /// </summary>
        public static string GetUnityPath(string path)
        {
            path = Path.GetFullPath(path); // normalize the path

            if (IsPathChild(assetsPath, path)) // it's in the "Assets" folder
            {
                return Path.Join(
                    Path.GetFileName(assetsPath),
                    Path.GetRelativePath(assetsPath, path)
                );
            }
            else if (IsPathChild(packagesPath, path)) // it's in the "Packages" folder
            {
                return Path.GetRelativePath(Path.GetDirectoryName(packagesPath), path);
            }

            throw new InvalidUnityPath(path);
        }

        /// <summary>
        /// Returns true if the given child path is located in any subdirectory of parent, or if it is located in parent itself.
        /// Returns false if the parent and child paths are the same, or if the child is not located within the parent.
        /// </summary>
        public static bool IsPathChild(string parent, string child)
        {
            // NullOrEmpty == root directory
            if ((parent == child) || (string.IsNullOrEmpty(parent) && string.IsNullOrEmpty(child))) return false;

            if (string.IsNullOrEmpty(parent)) return true;
            if (string.IsNullOrEmpty(child)) return false;

            // Although this method does not require realistic file paths, we must treat them as realistic paths for
            // the purposes of comparison.
            parent = Path.GetFullPath(parent);
            child = Path.GetFullPath(child);

            // First check if the two are even on the same disk
            if (parent.Contains(Path.VolumeSeparatorChar) && child.Contains(Path.VolumeSeparatorChar))
            {
                string parentVolume = parent[..(parent.IndexOf(Path.VolumeSeparatorChar) + 1)];
                string childVolume = child[..(child.IndexOf(Path.VolumeSeparatorChar) + 1)];
                if (parentVolume != childVolume) return false;
            }

            //Debug.Log(parent + " " + child);
            foreach (string path in IterateDirectories(child))
            {
                //Debug.Log(path);
                if (parent == path) return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the mouse is currently hovering over the given Rect, and false otherwise.
        /// </summary>
        public static bool IsMouseOverRect(Rect rect)
        {
            if (Event.current != null) return rect.Contains(Event.current.mousePosition) && GUI.enabled;
            return false;
        }

        /// <summary>
        /// When the mouse cursor is hovering over the given rect, the mouse cursor will change to the type specified.
        /// </summary>
        public static void SetCursorInRect(Rect rect, MouseCursor cursor) => EditorGUIUtility.AddCursorRect(rect, cursor);

        public static bool IsMouseButtonPressed() { if (Event.current == null) return false; return Event.current.rawType == EventType.MouseDown; }
        public static bool IsMouseButtonReleased() { if (Event.current == null) return false; return Event.current.rawType == EventType.MouseUp; }
        public static bool IsMouseDragging() { if (Event.current == null) return false; return Event.current.rawType == EventType.MouseDrag; }

        public static string ColorString(string text, string color)
        {
            if (string.IsNullOrEmpty(text)) return null;
            return "<color=" + color + ">" + text + "</color>";
        }

        [HideInCallstack]
        private static string GetLogString(string message, string color = null)
        {
            string tag = "<size=10>" + debugTag + "</size>";
            if (!string.IsNullOrEmpty(color)) tag = ColorString(tag, color);
            return string.Join(' ', tag, message);
        }


        /// <summary>
        /// Print a log message to the console, intended for debug messages.
        /// </summary>
        [HideInCallstack] public static void Log(string message, Object context, string color) => Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, context, "{0}", GetLogString(message, color));
        [HideInCallstack] public static void Log(string message, Object context) => Log(message, context, null);
        [HideInCallstack] public static void Log(string message, string color) => Log(message, null, color);
        [HideInCallstack] public static void Log(string message) => Log(message, null, null);

        /// <summary>
        /// Print a warning message to the console.
        /// </summary>
        [HideInCallstack] public static void LogWarning(string message, Object context, string color) => Debug.LogWarning(GetLogString(message, color), context);
        [HideInCallstack] public static void LogWarning(string message, Object context) => LogWarning(message, context, null);
        [HideInCallstack] public static void LogWarning(string message, string color) => LogWarning(message, null, color);
        [HideInCallstack] public static void LogWarning(string message) => LogWarning(message, null, null);


        /// <summary>
        /// Print a warning message to the console.
        /// </summary>
        [HideInCallstack] public static void LogError(string message, Object context, string color) => Debug.LogError(GetLogString(message, color), context);
        [HideInCallstack] public static void LogError(string message, Object context) => LogError(message, context, null);
        [HideInCallstack] public static void LogError(string message, string color) => LogError(message, null, color);
        [HideInCallstack] public static void LogError(string message) => LogError(message, null, null);

        /// <summary>
        /// Print an exception to the console. The color cannot be changed.
        /// </summary>
        [HideInCallstack] public static void LogException(System.Exception exception, Object context) => Debug.LogException(exception, context);
        [HideInCallstack] public static void LogException(System.Exception exception) => Debug.LogException(exception);
        


        /// <summary>
        /// Signifies that a path is not located in either the "Assets" or "Packages" folder of a project.
        /// </summary>
        public class InvalidUnityPath : System.Exception
        {
            public InvalidUnityPath() { }
            public InvalidUnityPath(string message) : base(message) { }
            public InvalidUnityPath(string message, System.Exception inner) : base(message, inner) { }
        }
    }
}