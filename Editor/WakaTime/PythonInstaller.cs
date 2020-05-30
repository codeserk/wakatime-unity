using UnityEditor;
using System;
using UnityEngine;
using System.Diagnostics;

using System.IO;

#pragma warning disable 612, 618

namespace WakaTime
{
	/// <summary>
	/// Python installer.
	/// </summary>
	public class PythonInstaller
	{
		/// <summary>
		/// HTTP client
		/// 
		/// TODO: Use the new client solution.
		/// </summary>
		static WWW www = null;

		/// <summary>
		/// Process that is installing python.
		/// </summary>
		static Process installProcess = null;

		/// <summary>
		/// Gets the application data folder.
		/// </summary>
		/// <returns>Folder</returns>
		static string GetFileFolder()
		{
			return System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
		}

		/// <summary>
		/// Gets python path
		/// </summary>
		/// <returns>Python path</returns>
		static string GetFilePath()
		{
			return GetFileFolder() + PythonManager.GetPythonFileName();
		}

		/// <summary>
		/// Returns whether python is downloaded.
		/// </summary>
		/// <returns>Whether python is downloaded</returns>
		static bool IsDownloaded()
		{
			return File.Exists(GetFilePath());
		}

		/// <summary>
		/// Downloads and installs pyhton.
		/// </summary>
		static public void DownloadAndInstall()
		{
			if (!PythonManager.IsPythonInstalled())
			{
				if (!IsDownloaded())
				{
					Download();
				}
				else
				{
					Install();
				}
			}
		}

		/// <summary>
		/// Returns whether python is being installed.
		/// </summary>
		/// <returns>Whether python is being installed</returns>
		static public bool IsInstalling()
		{
			return IsDownloading() || installProcess != null;
		}

		/// <summary>
		/// Returns whether python is being downloaded.
		/// </summary>
		/// <returns>Whether python is being downloaded</returns>
		public static bool IsDownloading()
		{
			return www != null;
		}

		/// <summary>
		/// Downloads python.
		/// </summary>
		static public void Download()
		{
			string url = PythonManager.GetPythonDownloadUrl();

			www = new WWW(url);
			EditorApplication.update = WhileDownloading;
		}

		/// <summary>
		/// Function to call while python is being downloaded.
		/// </summary>
		static void WhileDownloading()
		{
			EditorUtility.DisplayProgressBar("Downloading Python", "Python is being downloaded", www.progress);

			if (www.isDone)
			{
				EditorApplication.update = null;
				DownloadCompleted();
			}
		}

		/// <summary>
		/// Handles download completion.
		/// Saves python to the filesystem and installs it.
		/// </summary>
		static void DownloadCompleted()
		{
			EditorUtility.ClearProgressBar();

			if (Main.IsDebug)
			{
				UnityEngine.Debug.Log("Python downloaded: " + www.bytesDownloaded.ToString());
			}
			string dir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
			string localFile = dir + PythonManager.GetPythonFileName();


			try
			{
				System.IO.FileStream stream = new System.IO.FileStream(localFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
				stream.Write(www.bytes, 0, www.bytes.Length);

				// close file stream
				stream.Close();

				www = null;
			}
			catch (Exception ex)
			{
				if (Main.IsDebug)
				{
					UnityEngine.Debug.LogError("Python download failed: " + ex.Message);
				}
			}

			Install();
		}

		/// <summary>
		/// Installs pyhton.
		/// </summary>
		static void Install()
		{
			string arguments = "/i \"" + GetFilePath() + "\"";
			arguments = arguments + " /norestart /qb!";

			try
			{
				var procInfo = new ProcessStartInfo
				{
					UseShellExecute = false,
					RedirectStandardError = true,
					FileName = "msiexec",
					CreateNoWindow = true,
					Arguments = arguments
				};

				installProcess = Process.Start(procInfo);
				installProcess.WaitForExit();
				installProcess.Close();

				installProcess = null;
			}
			catch (Exception ex)
			{
				if (Main.IsDebug)
				{
					UnityEngine.Debug.LogError("Python installation failed: " + ex.Message);
				}
			}
		}
	}
}

#pragma warning restore 612, 618