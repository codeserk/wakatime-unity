using System.Diagnostics;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace WakaTime
{
	/// <summary>
	/// Client request.
	/// Contains the information needed to make 1 wakatime heartbeat.
	/// </summary>
	public readonly struct ClientRequest
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="apiKey"></param>
		/// <param name="file"></param>
		public ClientRequest(string apiKey, string file)
		{
			this.apiKey = apiKey;
			this.file = file;
		}

		/// <summary>
		/// WakaTime API Key
		/// </summary>
		public readonly string apiKey;

		/// <summary>
		/// File to sned to wakatime.
		/// </summary>
		public readonly string file;
	}

	/// <summary>
	/// Handles operations with wakatime client.
	/// </summary>
	public class ClientManager
	{
		/// <summary>
		/// Requests stacks
		/// </summary>
		protected static readonly Queue<ClientRequest> requests = new Queue<ClientRequest>();

		/// <summary>
		/// Number of active requests.
		/// </summary>
		protected static int activeRequests = 0;

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
		public async static Task HeartBeat(string apiKey, string file, bool write = false)
		{
			if (activeRequests < Main.MaxRequests)
			{
				activeRequests++;

				if (Main.IsDebug)
				{
					UnityEngine.Debug.Log("[wakatime] Making request: " + activeRequests + " / " + Main.MaxRequests + " (enqueued: " + requests.Count + ")");
				}

				var dataPath = Application.dataPath;
				var projectName = Main.GetProjectName();
				var clientPath = GetClientPath();
				await Task.Run(() => MakeRequest(apiKey, file, projectName, dataPath, clientPath));

				activeRequests--;
				if (requests.Count > 0)
				{
					var request = requests.Dequeue();
					await HeartBeat(request.apiKey, request.file);
				}
			}
			else
			{
				if (Main.IsDebug)
				{
					UnityEngine.Debug.Log("[wakatime] Request enqueued (" + requests.Count + ")");
				}

				requests.Enqueue(new ClientRequest(apiKey, file));
			}
		}

		/// <summary>
		/// Sends a heart-beat to wakatime.
		/// Only works if the client is not installed.
		/// </summary>
		/// <param name="apiKey"></param>
		/// <param name="projectName"></param>
		/// <param name="file"></param>
		/// <param name="dataPath"></param>
		/// <param name="clientPath"></param>
		protected static void MakeRequest(string apiKey, string file, string projectName, string dataPath, string clientPath)
		{
			try
			{
				string arguments = "--key " + apiKey +
					" --file " + "\"" + file + "\"" +
					" --plugin " + WakaTimeConstants.PLUGIN_NAME +
					" --project " + "\"" + projectName + "\"" +
					" --verbose";

				if (Main.IsDebug)
				{
					UnityEngine.Debug.Log("[wakatime] Sending file: " + file);
				}

				Process p = new Process
				{
					StartInfo = {
						FileName = PythonManager.GetPythonPath (),
						Arguments = "\"" + clientPath + "\" " + arguments,
						CreateNoWindow = true,
						WindowStyle = ProcessWindowStyle.Hidden,
						WorkingDirectory = dataPath,
						UseShellExecute = false,
						RedirectStandardOutput = true,
						RedirectStandardError = true,
					}
				};

				p.Start();
				p.WaitForExit(5000);

				UnityEngine.Debug.Log("[wakatime] Finished sending file " + file);
			}
			catch (Exception ex)
			{
				if (Main.IsDebug)
				{
					UnityEngine.Debug.LogError("[wakatime] Error found while sending heartbeat to wakatime for file " + file + ": " + ex);
				}
			}
		}
	}
}