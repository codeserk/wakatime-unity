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

		public static string GetClientPath() {
			return Path.Combine(BaseDir, ClientPath);            
		}

		public static bool IsClientInstalled() {
			return File.Exists(GetClientPath());
		}

		public static bool IsClientLatestVersion() {
			if (!IsClientInstalled()) {
				return false;
			}

			return true;
		}

		public static void HeartBeat(string apiKey, string file, bool write = false) {
			if (PythonManager.IsPythonInstalled()) {
				string arguments = "--key " + apiKey
                                   + " --file " + "\"" + file + "\""
				                   + " --plugin " + WakaTimeConstants.PLUGIN_NAME
				                   + " --project " + Main.GetProjectName()
				                   + " --verbose";

				if (Main.IsDebug) {
					UnityEngine.Debug.Log("Sending file: " + PythonManager.GetPythonPath() + " " + GetClientPath() + " " + arguments);
				}

				Process p = new Process();
				p.StartInfo.FileName = PythonManager.GetPythonPath();
                p.StartInfo.Arguments = "\"" + GetClientPath() + "\" " + arguments;  
		
				p.StartInfo.CreateNoWindow = true;
				p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

				p.StartInfo.WorkingDirectory = Application.dataPath; 

#if UNITY_EDITOR_WIN
				p.StartInfo.UseShellExecute = true;
#else
				p.StartInfo.UseShellExecute = true;
#endif


				p.Start();

//			UnityEngine.Debug.Log (p.StandardOutput.ReadToEnd ());
//			UnityEngine.Debug.Log (p.StandardError.ReadToEnd ());

				p.Close();
			}
		}
	}

}