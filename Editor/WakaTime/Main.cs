using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

using System;

using System.Collections.Generic;

namespace WakaTime
{
	/// <summary>
	/// Main class for the plugin.
	/// </summary>
	[InitializeOnLoad]
	public class Main
	{
		/// <summary>
		/// Key in editor preferences to know whether wakatime is enabled or not.
		/// </summary>
		const string KEY_ENABLED = "wakatime_enabled";

		/// <summary>
		/// Key in editor preferences to know whether debug mode is enabled or not.
		/// </summary>
		const string KEY_DEBUG = "wakatime_debug";

		/// <summary>
		/// Key in editor preferences to get the API key.
		/// </summary>
		public const string KEY_API_KEY = "wakatime_api_key";

		/// <summary>
		/// Key in editor preferences to get max requests.
		/// </summary>
		public const string KEY_MAX_REQUESTS = "wakatime_max_requests";

		/// <summary>
		/// Name of the current scene.
		/// </summary>
		private static string currentScene;

		/// <summary>
		/// API Key
		/// </summary>
		static string _apiKey = null;

		/// <summary>
		/// API Key wrapper.
		/// Gets the API key from the editor preferences if it's not set.
		/// Saves the key in the editor preferences when it's set.
		/// </summary>
		public static string ApiKey
		{
			get
			{
				if (_apiKey == null)
				{
					_apiKey = EditorPrefs.GetString(KEY_API_KEY, "");
				}

				return _apiKey;
			}
			set
			{
				_apiKey = value;
				EditorPrefs.SetString(KEY_API_KEY, value);

				CheckAPIKey();
			}
		}

		/// <summary>
		/// Whether the plugin is enabled.
		/// </summary>
		static bool _enabled = false;

		/// <summary>
		/// Enabled wrapper
		/// Saves whether it's enabled in the editor preferences if a new value is set.
		/// </summary>
		public static bool IsEnabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
				EditorPrefs.SetBool(KEY_ENABLED, value);

				if (value)
				{
					Check();
				}
			}
		}

		/// <summary>
		/// Whether the debug mode is enabled or not.
		/// </summary>
		static Boolean _debug = false;

		/// <summary>
		/// Debug wrapper
		/// Saves whether debug is enabled in the editor preferences if a new value is set.
		/// </summary>
		public static bool IsDebug
		{
			get
			{
				return _debug;
			}
			set
			{
				_debug = value;
				EditorPrefs.SetBool(KEY_DEBUG, value);
			}
		}

		/// <summary>
		/// Maximun number of concurrent requests to wakatime.
		/// 5 by default.
		/// </summary>
		static int? _maxRequests = null;

		/// <summary>
		/// Max requests wrapper.
		/// Gets the value from the editor preferences if it's not set.
		/// Saves the new value in the editor preferences.
		/// </summary>
		public static int MaxRequests
		{
			get
			{
				if (_maxRequests == null)
				{
					_maxRequests = EditorPrefs.GetInt(KEY_API_KEY, 5);
				}

				return (int)_maxRequests;
			}
			set
			{
				_maxRequests = value;
				EditorPrefs.SetInt(KEY_MAX_REQUESTS, value);
			}
		}

		/// <summary>
		/// Last times when the files changed.
		/// {file path} => {last updated at}
		/// </summary>
		static readonly Dictionary<string, DateTime> fileTimes = new Dictionary<string, DateTime>();

		/// <summary>
		/// Gets the project path.
		/// </summary>
		/// <returns>Path</returns>
		public static string GetProjectPath()
		{
			return Application.dataPath + "/../";
		}

		/// <summary>
		/// Initializes the plugin.
		/// </summary>
		static Main()
		{
			_enabled = EditorPrefs.GetBool(KEY_ENABLED, true);
			_debug = EditorPrefs.GetBool(KEY_DEBUG, false);

			currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
			EditorApplication.hierarchyChanged += OnWindowChanged;

			Check();
		}

		/// <summary>
		/// Handles event when the window changes.
		/// Checks the current scene and updates it if it changed.
		/// </summary>
		private static void OnWindowChanged()
		{
			if (currentScene != UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
			{
				currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

				// Current scene changed
				OnSceneChanged(GetProjectPath() + currentScene);
			}
		}

		/// <summary>
		/// Checks the plugin:
		/// - Whether the API key is set.
		/// - Whether the python client is installed.
		/// </summary>
		/// <returns>Whether the plugin can be used.</returns>
		public static bool Check()
		{
			bool res = false;

			// Checking python client is expensive, so only do if the API key is set.
			if (CheckAPIKey())
			{
				res = CheckPython();
			}

			return res;
		}

		/// <summary>
		/// Checks the API key.
		/// </summary>
		/// <returns>Whether the API key is valid.</returns>
		public static bool CheckAPIKey()
		{
			bool res = true;
			string key = GetApiKey();

			if (IsEnabled)
			{
				if (key == null || key.Equals(""))
				{
					if (!Window.IsFocused())
					{
						if (EditorUtility.DisplayDialog("WakaTime API Key required", "You need to insert your API Key to use this Plugin", "Insert API Key", "Disable Wakatime"))
						{
							Window.GetWindow().Show();
							Window.GetWindow().Focus();
							IsEnabled = false;
						}
						else
						{
							IsEnabled = false;
						}
					}

					res = false;
				}
			}
			else
			{
				res = false;
			}

			return res;
		}

		/// <summary>
		/// Checks whether python is installed.
		/// </summary>
		/// <returns>Whether python is installed.</returns>
		public static bool CheckPython()
		{
			bool isInstalled = PythonManager.IsPythonInstalled();

			if (IsEnabled && !isInstalled && !PythonInstaller.IsInstalling())
			{
				if (EditorUtility.DisplayDialog("Python is required", "The plugin is about to install Python. Do you want to continue?", "Install Python", "Disable Wakatime"))
				{
					PythonInstaller.DownloadAndInstall();
				}
				else
				{
					IsEnabled = false;
				}
			}

			return isInstalled;
		}

		/// <summary>
		/// Gets the project name.
		/// </summary>
		/// <returns>Project name</returns>
		public static string GetProjectName()
		{
			string[] s = Application.dataPath.Split('/');
			string projectName = s[s.Length - 2];

			return projectName;
		}

		/// <summary>
		/// Gets the API key
		/// </summary>
		/// <returns>API key</returns>
		public static string GetApiKey()
		{
			return EditorPrefs.GetString(KEY_API_KEY);
		}

		/// <summary>
		/// Handles scene change event.
		/// </summary>
		/// <param name="path">Scene path</param>
		public static void OnSceneChanged(string path)
		{
			RequestSendFile(path, false);
		}

		/// <summary>
		/// Handles asset change event.
		/// </summary>
		/// <param name="path">Asset path</param>
		public static void OnAssetChanged(string path)
		{
			RequestSendFile(path, false);
		}

		/// <summary>
		/// Handles asset saved event.
		/// </summary>
		/// <param name="path">Asset path</param>
		public static void OnAssetSaved(string path)
		{
			RequestSendFile(path, true);
		}

		/// <summary>
		/// Request notify file change to wakatime.
		///
		/// TODO: Check what was `write` about.
		/// </summary>
		/// <param name="path">File path</param>
		/// <param name="write"></param>
		static async void RequestSendFile(string path, bool write = false)
		{
			if (Check() && ShouldSendFile(path))
			{
				await ClientManager.HeartBeat(GetApiKey(), path, write);
			}
		}

		/// <summary>
		/// Checks whether the file should be sent or not.
		/// </summary>
		/// <param name="path">File path</param>
		/// <returns>Whether the file should be sent or not.</returns>
		static bool ShouldSendFile(string path)
		{
			// Whether the file changed was inside this plugin.
			if (path.Contains("Assets/Editor/WakaTime"))
			{
				return false;
			}

			bool shouldSendFile = true;
			if (fileTimes.ContainsKey(path))
			{
				DateTime time;

				fileTimes.TryGetValue(path, out time);

				double diffInSeconds = (DateTime.Now - time).TotalSeconds;

				if (diffInSeconds < WakaTimeConstants.TIME_TO_HEARTBEAT)
				{
					shouldSendFile = false;
				}
			}

			if (shouldSendFile)
			{
				// Update current time
				if (fileTimes.ContainsKey(path))
				{
					fileTimes.Remove(path);
				}

				fileTimes.Add(path, DateTime.Now);
			}

			if (IsEnabled && IsDebug)
			{
				Debug.Log("[wakatime] Should send " + path.Substring(path.LastIndexOf("/") + 1) + "? [" + (shouldSendFile ? "yes" : "no") + "]");
			}

			return true;
		}
	}
}
