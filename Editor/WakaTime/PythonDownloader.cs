using UnityEditor;
using System;
using UnityEngine;
using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Collections;

using System.IO;

namespace WakaTime {
	public class PythonDownloader {
		static WWW www;

		static public void Download () {
			string url = PythonManager.GetPythonDownloadUrl ();
			string dir = ClientManager.BaseDir;
		
			www = new WWW (url);
			EditorApplication.update = WaitingToEnd;
		}

		static void WaitingToEnd () {
			string downloadingStr = "Downloading Python [";
			for (int i = 0; i < 10; i++) {
				int iProgress = (int)(www.progress * 10f);

				if (i < iProgress) {
					downloadingStr += "-";
				} else {
					downloadingStr += "_";
				}
			}
			downloadingStr += "]";
			UnityEngine.Debug.Log (downloadingStr);

			if (www.isDone) {
				EditorApplication.update = null;
				DownloadCompleted ();
			}

		}

		static void DownloadCompleted () {
			UnityEngine.Debug.Log ("Python downloaded: " + www.size.ToString ());
			string dir = ClientManager.BaseDir;
			string localFile = dir + PythonManager.GetPythonFileName ();

			try {
				System.IO.FileStream stream = new System.IO.FileStream (localFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
				stream.Write (www.bytes, 0, www.bytes.Length);
					
				// close file stream
				stream.Close ();

				UnityEngine.Debug.Log ("File saved!");
			} catch (Exception ex) {

				UnityEngine.Debug.LogError (ex);
			}

//			ZipManager.Decompress (www.bytes, localZipFile);

		}
	}
}