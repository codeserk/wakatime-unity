using UnityEditor;

namespace WakaTime
{
	/// <summary>
	/// Asset postprocessor to act when some assets have changed.
	/// </summary>
	public class AssetChangedChecker : AssetPostprocessor
	{
		/// <summary>
		/// Subscribes to assets modifications.
		/// </summary>
		/// <param name="importedAssets"></param>
		/// <param name="deletedAssets"></param>
		/// <param name="movedAssets"></param>
		/// <param name="movedFromAssetPaths"></param>
		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			foreach (var str in importedAssets)
			{
				Main.OnAssetSaved(Main.GetProjectPath() + str);
			}
			foreach (var str in deletedAssets)
			{
				Main.OnAssetChanged(Main.GetProjectPath() + str);
			}

			foreach (var str in movedAssets)
			{
				Main.OnAssetChanged(Main.GetProjectPath() + str);
			}
			foreach (var str in movedFromAssetPaths)
			{
				Main.OnAssetChanged(Main.GetProjectPath() + str);
			}
		}
	}

}