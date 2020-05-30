using UnityEngine;
using UnityEditor;

namespace WakaTime
{
	/// <summary>
	/// WakaTime plugin window.
	/// </summary>
	public class Window : EditorWindow
	{
		/// <summary>
		/// Url to wakatime
		/// </summary>
		const string WAKATIME_URL = "https://wakatime.com/";

		/// <inheritdoc/>
		[MenuItem("Window/WakaTime")]
		public static void ShowWindow()
		{
			EditorWindow.GetWindow(typeof(Window));
		}

		/// <inheritdoc/>
		void OnGUI()
		{
			GUILayout.Label("WakaTime configuration", EditorStyles.boldLabel);

			if (GUILayout.Button("Visit " + WAKATIME_URL))
			{
				Application.OpenURL(WAKATIME_URL);
			}

			EditorGUILayout.Separator();

			Main.IsEnabled = EditorGUILayout.Toggle("Enabled", Main.IsEnabled);
			EditorGUILayout.Separator();


			Main.ApiKey = EditorGUILayout.TextField("API key", Main.ApiKey);
			if (Main.IsEnabled && Main.ApiKey == null || "".Equals(Main.ApiKey))
			{
				EditorGUILayout.HelpBox("API Key is required", MessageType.Error, false);
			}
			Main.MaxRequests = EditorGUILayout.IntField("Max Requests", Main.MaxRequests);
			EditorGUILayout.HelpBox("Maximum number of simultaneous requests to wakatime. Be cautious when increasing this value, since that might cause CPU problems.", MessageType.Info, true);

			EditorGUILayout.Separator();

			Main.IsDebug = EditorGUILayout.Toggle("Debug", Main.IsDebug);
			EditorGUILayout.HelpBox("Debug messages will appear in the console if this option is enabled. Mostly used for test purposes.", MessageType.Info, true);
		}

		/// <inheritdoc/>
		public static bool IsFocused()
		{
			return EditorWindow.focusedWindow is Window;
		}

		/// <inheritdoc/>
		public static EditorWindow GetWindow()
		{
			return EditorWindow.GetWindow(typeof(Window));
		}
	}
}