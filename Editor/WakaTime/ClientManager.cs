using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace WakaTime
{
	/// <summary>
	/// Handles operations with wakatime client.
	/// </summary>
	public class ClientManager
	{
		/// <summary>
		/// Gets the base directory where wakatime is installed.
		/// </summary>
		public static string BaseDir
		{
			get { return Application.dataPath + "/Editor/WakaTime/"; }
		}

		/// <summary>
		/// Gets the path to wakatime python client.
		/// </summary>
		public static string ClientPath
		{
			get { return @"client/wakatime/cli.py"; }
		}

		/// <summary>
		/// Gets the path to wakatime python client, including root directory.
		/// </summary>
		/// <returns>Path</returns>
		public static string GetClientPath()
		{
			return Path.GetFullPath(Path.Combine(BaseDir, ClientPath));
		}

		/// <summary>
		/// Checks whether the client is installed.
		/// </summary>
		/// <returns>Whether the client is installed</returns>
		public static bool IsClientInstalled()
		{
			return File.Exists(GetClientPath());
		}

		/// <summary>
		/// Whether the client has the latest version.
		/// </summary>
		/// <remarks>This method is not implemented.</remarks>
		/// <returns></returns>
		public static bool IsClientLatestVersion()
		{
			if (!IsClientInstalled())
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Sends a heart-beat to wakatime.
		/// Only works if the client is not installed.
		/// </summary>
		/// <param name="apiKey"></param>
		/// <param name="file"></param>
		/// <param name="write"></param>
		public static void HeartBeat(string apiKey, string file, bool write = false)
		{
			if (!PythonManager.IsPythonInstalled()) return;

			string arguments = "--key " + apiKey +
				" --file " + "\"" + file + "\"" +
				" --plugin " + WakaTimeConstants.PLUGIN_NAME +
				" --project " + "\"" + Main.GetProjectName() + "\"" +
				" --verbose";

			if (Main.IsDebug)
			{
				UnityEngine.Debug.Log("[wakatime] Sending file: " + PythonManager.GetPythonPath() + " " + GetClientPath() +
					" " + arguments);
			}

			Process p = new Process
			{
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

			p.Start();

			if (Main.IsDebug)
			{
				var output = p.StandardOutput.ReadToEnd();
				if (output.Length > 0)
				{
					UnityEngine.Debug.Log("[wakatime] Output: " + output);
				}

				var errors = p.StandardError.ReadToEnd();
				if (errors.Length > 0)
				{
					UnityEngine.Debug.LogError("[wakatime] Error: " + errors);
				}
			}

			p.Close();
		}
	}
}