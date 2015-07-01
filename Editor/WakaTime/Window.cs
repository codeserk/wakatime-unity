using UnityEngine;
using System.Collections;
using UnityEditor;

namespace WakaTime {
	public class Window : EditorWindow {
		const string WAKATIME_URL = "https://wakatime.com/";

		[MenuItem ("Window/WakaTime")]
		public static void ShowWindow () {
			EditorWindow.GetWindow (typeof(Window));
		}
		
		void OnGUI () {
			GUILayout.Label ("WakaTime configuration", EditorStyles.boldLabel);

			if (GUILayout.Button("Visit " + WAKATIME_URL)) {
				Application.OpenURL(WAKATIME_URL);
			}

			EditorGUILayout.Separator ();

			Main.IsEnabled = EditorGUILayout.Toggle ("Enabled", Main.IsEnabled);
			EditorGUILayout.Separator ();


			Main.ApiKey = EditorGUILayout.TextField ("API key", Main.ApiKey);
			if (Main.IsEnabled && Main.ApiKey == null || "".Equals (Main.ApiKey)) {
				EditorGUILayout.HelpBox ("API Key is required", MessageType.Error, false);
			}	
		
			EditorGUILayout.Separator ();

			Main.IsDebug = EditorGUILayout.Toggle ("Debug", Main.IsDebug);
			EditorGUILayout.HelpBox ("Debug messages will appear in the console if this option is enabled. Mostly used for test purposes.", MessageType.Info, true);
		}

		public static bool IsFocused () {
			return EditorWindow.focusedWindow is Window;
		}
		
		public static EditorWindow GetWindow () {
			return EditorWindow.GetWindow (typeof(Window));
		}
	}
}