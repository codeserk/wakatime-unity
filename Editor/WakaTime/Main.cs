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
		const string KEY_ENABLED = "wakatime_enabled";
		const string KEY_DEBUG = "wakatime_debug";
		public const string KEY_API_KEY = "wakatime_api_key";

		private static string currentScene;


		static string _apiKey = null;
		public static string ApiKey {
			get {
				if(_apiKey == null) {
					_apiKey = EditorPrefs.GetString(KEY_API_KEY, "");
				}

				return _apiKey;
			}
			set {
				_apiKey = value;
				EditorPrefs.SetString(KEY_API_KEY, value);

				CheckAPIKey();
			}
		}

		static bool _enabled = false;
		public static bool IsEnabled {
			get {
				return _enabled;
			}
			set {
				_enabled = value;
				EditorPrefs.SetBool(KEY_ENABLED, value);

				if(value) {
					Check();
				}
			}
		}

		static Boolean _debug = false;
		public static bool IsDebug {
			get {				
				return _debug;
			}
			set {
				_debug = value;
				EditorPrefs.SetBool(KEY_DEBUG, value);
			}
		}

		static Dictionary<string, DateTime> fileTimes = new Dictionary<string, DateTime> ();

		public static string GetProjectPath () {
			return Application.dataPath + "/../";
		}

		static Main () {
			_enabled = EditorPrefs.GetBool (KEY_ENABLED, true);
			_debug = EditorPrefs.GetBool (KEY_DEBUG, false);

			currentScene = EditorApplication.currentScene;
			EditorApplication.hierarchyWindowChanged += OnWindowChanged;

			Check ();
		}



		private static void OnWindowChanged () {
			if (currentScene != EditorApplication.currentScene) {
				currentScene = EditorApplication.currentScene;

				// Current scene changed
				OnSceneChanged (GetProjectPath () + currentScene);
			}
		}

		public static bool Check() {
			bool res = false;

			if (CheckAPIKey ()) {
				res = CheckPython();
			}

			return res;
		}

		public static bool CheckAPIKey () {
			bool res = true;
			string key = GetApiKey ();

			if(IsEnabled) {
				if (key == null || key.Equals ("")) {
					if (!Window.IsFocused ()) {
						if (EditorUtility.DisplayDialog ("WakaTime API Key required", "You need to insert your API Key to use this Plugin", "Insert API Key", "Disable Wakatime")) {
						 	Window.GetWindow ().Show ();
							Window.GetWindow ().Focus ();
						} else {
							IsEnabled = false;
						}
					}

					res = false;
				}
			} else {
				res = false;
			}

			return res;
		}

		public static bool CheckPython() {
			bool isInstalled = PythonManager.IsPythonInstalled ();

			if (IsEnabled && !isInstalled && !PythonInstaller.IsInstalling()) {
				if (EditorUtility.DisplayDialog ("Python is required", "The plugin is about to install Python. Do you want to continue?", "Install Python", "Disable Wakatime")) {
					PythonInstaller.DownloadAndInstall ();
				} else {
					IsEnabled = false;
				}
			}

			return isInstalled;
		}

		public static string GetProjectName () {
			string[] s = Application.dataPath.Split ('/');
			string projectName = s [s.Length - 2];

			return projectName;
		}

		public static string GetApiKey () {
			return EditorPrefs.GetString (KEY_API_KEY);
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
			if (Check() && ShouldSendFile (path)) {
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

				double diffInSeconds = (DateTime.Now - time).TotalSeconds;

				if (diffInSeconds < WakaTimeConstants.TIME_TO_HEARTBEAT) {
					res = false;
				}
			}

			if (res) {
				// update current time
				if (fileTimes.ContainsKey (path)) {
					fileTimes.Remove (path);
				}

				fileTimes.Add (path, DateTime.Now);
			}

			if (IsEnabled && IsDebug) {
				Debug.Log ("Should send " + path.Substring(path.LastIndexOf("/") + 1) + "? [" + (res ? "yes" : "no") + "]");
			}

			return res;
		}
	}
}
