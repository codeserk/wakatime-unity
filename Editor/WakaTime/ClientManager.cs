using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace WakaTime {
	public class ClientManager {
		public static string BaseDir {
			get { return Application.dataPath + "/Editor/WakaTime/"; }
		}

		public static string ClientPath {
			get { return @"client/wakatime/cli.py"; }
		}

		public static string GetClientPath () {
			return Path.GetFullPath (Path.Combine (BaseDir, ClientPath));
		}

		public static bool IsClientInstalled () {
			return File.Exists (GetClientPath ());
		}

		public static bool IsClientLatestVersion () {
			if (!IsClientInstalled ()) {
				return false;
			}

			return true;
		}

		public static void HeartBeat (string apiKey, string file, bool write = false) {
			if (!PythonManager.IsPythonInstalled ()) return;

			string arguments = "--key " + apiKey +
				" --file " + "\"" + file + "\"" +
				" --plugin " + WakaTimeConstants.PLUGIN_NAME +
				" --project " + Main.GetProjectName () +
				" --verbose";

			if (Main.IsDebug) {
				UnityEngine.Debug.Log ("Sending file: " + PythonManager.GetPythonPath () + " " + GetClientPath () +
					" " + arguments);
			}

			Process p = new Process {
				StartInfo = {
					FileName = PythonManager.GetPythonPath (),
					Arguments = "\"" + GetClientPath () + "\" " + arguments,
					CreateNoWindow = true,
					WindowStyle = ProcessWindowStyle.Hidden,
					WorkingDirectory = Application.dataPath,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				}
			};

			p.Start ();

			if (Main.IsDebug) {
				var output = p.StandardOutput.ReadToEnd ();
				if (output.Length > 0) {
					UnityEngine.Debug.Log ("Wakatime Output: " + output);
				}

				var errors = p.StandardError.ReadToEnd ();
				if (errors.Length > 0) {
					UnityEngine.Debug.LogError ("Wakatime Error: " + errors);
				}
			}

			p.Close ();
		}
	}
}