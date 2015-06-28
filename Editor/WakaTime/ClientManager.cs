using UnityEngine;
using System.Collections;
using System.IO;
using System.Diagnostics;

namespace WakaTime {
	public class ClientManager {
		public static string BaseDir {
			get { 
				return Application.dataPath + "/Editor/WakaTime/";
			}
		}

		public static string ClientPath {
			get {
				return @"client/wakatime/cli.py";
			}
		}

		public static string GetClientPath () {
			return Path.Combine (BaseDir, ClientPath);            
		}

		public static bool IsClientInstalled () {
			UnityEngine.Debug.Log ("path:" + GetClientPath () + ": " + File.Exists (GetClientPath ()));

			return File.Exists (GetClientPath ());
		}
	
		public static bool IsClientLatestVersion () {
			if (!IsClientInstalled ()) {
				return false;
			}

			return true;

			ProcessStartInfo info = new ProcessStartInfo (PythonManager.GetPythonPath () + " " + GetClientPath (), "--version");
			Process process = new Process ();
			process.StartInfo = info;

			if (process.Start ()) {

				string error = process.StandardOutput.ReadLine ();
				process.WaitForExit ();

				UnityEngine.Debug.Log ("Error:" + error);

				return error.Contains (WakaTimeConstants.CURRENT_CLIENT_VERSION);
			} else {
				return false;
			}
		}

		public static void HeartBeat (string apiKey, string file, bool write = false) {
			string arguments = "--key " + apiKey 
				+ " --file " + file
				+ " --plugin " + WakaTimeConstants.PLUGIN_NAME
				+ " --project " + Main.GetProjectName ()
				+ " --verbose";

			UnityEngine.Debug.Log (PythonManager.GetPythonPath () + " " + GetClientPath () + " " + arguments);

			Process p = new Process ();
			p.StartInfo.FileName = PythonManager.GetPythonPath ();
			p.StartInfo.Arguments = GetClientPath () + " " + arguments;    
		
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

			p.StartInfo.WorkingDirectory = Application.dataPath; 
			p.StartInfo.UseShellExecute = false;

			p.Start ();

//			UnityEngine.Debug.Log (p.StandardOutput.ReadToEnd ());
//			UnityEngine.Debug.Log (p.StandardError.ReadToEnd ());

			p.Close ();
		}
	}

}