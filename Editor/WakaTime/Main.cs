using UnityEngine;
using UnityEditor;
using System.Collections;

using System;
using System.IO.Compression;
using System.Net;

using System.Collections.Generic;

namespace WakaTime {
	[InitializeOnLoad]
	public class Main {
		private static string currentScene;

		static Dictionary<string, DateTime> fileTimes = new Dictionary<string, DateTime> ();

		public static string GetProjectPath () {
			return Application.dataPath + "/../";
		}

		static Main () {
			currentScene = EditorApplication.currentScene;
			EditorApplication.hierarchyWindowChanged += OnWindowChanged;
			
			if (!PythonManager.IsPythonInstalled ()) {
				PythonDownloader.Download ();
			}

			CheckAPI ();
		}

		private static void OnWindowChanged () {
			if (currentScene != EditorApplication.currentScene) {
				currentScene = EditorApplication.currentScene;

				// Current scene changed
				OnSceneChanged (GetProjectPath () + currentScene);
			}
		}
		
		public static bool CheckAPI () {
			bool res = true;
			string key = GetApiKey ();


			if (key == null || key.Equals ("")) {
				if (!Window.IsFocused ()) {
					if (EditorUtility.DisplayDialog ("WakaTime Api needed.", "You need to insert your API Key so as tu use the Plugin.", "Insert API")) {
						Window.GetWindow ().Show ();
						Window.GetWindow ().Focus ();
					}
				}

				res = false;
			}

			return res;
		}

		public static string GetProjectName () {
			string[] s = Application.dataPath.Split ('/');
			string projectName = s [s.Length - 2];

			return projectName;
		}

		public static string GetApiKey () {
			return EditorPrefs.GetString ("wakatime_api_key");
		}

		public static void OnSceneChanged (string path) {
			RequestSendFile (path, false);
		}

		public static void OnAssetChanged (string path) {
			RequestSendFile (path, false);
		}

		public static void OnAssetSaved (string path) {
			RequestSendFile (path, true);
		}

		static void RequestSendFile (string path, bool write = false) {
			if (CheckAPI () && ShouldSendFile (path)) {
				ClientManager.HeartBeat (GetApiKey (), path, write);
			}
		}

		static bool ShouldSendFile (string path) {
			// Contains this Plugin?
			if (path.Contains ("Assets/Editor/WakaTime")) {
				return false;
			}

			bool res = true;
			if (fileTimes.ContainsKey (path)) {
				DateTime time;

				fileTimes.TryGetValue (path, out time);

				if (time != null) {
					double diffInSeconds = (DateTime.Now - time).TotalSeconds;

					if (diffInSeconds < WakaTimeConstants.TIME_TO_HEARTBEAT) {
						res = false;
					}
				}
			}

			if (res) {
				// update current time
				if (fileTimes.ContainsKey (path)) {
					fileTimes.Remove (path);
				}

				fileTimes.Add (path, DateTime.Now);
			}

			Debug.Log ("Should? " + path + "[" + res + "]");

			return res;
		}
	}
}
