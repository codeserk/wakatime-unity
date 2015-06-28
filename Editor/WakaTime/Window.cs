using UnityEngine;
using System.Collections;
using UnityEditor;

namespace WakaTime {
	public class Window : EditorWindow {
		string apiKey;

		[MenuItem ("Window/WakaTime")]
		public static void ShowWindow () {
			EditorWindow.GetWindow (typeof(Window));
		}
		
		void OnGUI () {
			GUILayout.Label ("WakaTime configuration", EditorStyles.boldLabel);
			
			if (apiKey == null || "".Equals (apiKey)) {
				GUILayout.Label ("Api key needs to be set.");
			}
			apiKey = EditorGUILayout.TextField ("Api key", apiKey);
			
			EditorPrefs.SetString ("wakatime_api_key", apiKey);
		}
		
		void OnEnable () {
			apiKey = Main.GetApiKey ();
		}

		public static bool IsFocused () {
			return EditorWindow.focusedWindow is Window;
		}
		
		public static EditorWindow GetWindow () {
			return EditorWindow.GetWindow (typeof(Window));
		}
	}
}