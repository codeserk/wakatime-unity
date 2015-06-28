using UnityEngine;
using UnityEditor;
using System.Collections;

namespace WakaTime {
	public class AssetChangedChecker :  AssetPostprocessor {
		static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
			foreach (var str in importedAssets) {
				Main.OnAssetSaved (Main.GetProjectPath () + str);

//				Debug.Log ("Reimported Asset: " + str);

			}
			foreach (var str in deletedAssets) {
				Main.OnAssetChanged (Main.GetProjectPath () + str);

			}
		
			foreach (var str in movedAssets) {
				Main.OnAssetChanged (Main.GetProjectPath () + str);
			}
			foreach (var str in movedFromAssetPaths) {
				Main.OnAssetChanged (Main.GetProjectPath () + str);
			}
		}
	}

}