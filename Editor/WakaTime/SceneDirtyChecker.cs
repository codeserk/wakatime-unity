using UnityEditor;
using UnityEditor.SceneManagement;

namespace WakaTime
{
	/// <summary>
	/// Checks if the scene is dirty.
	/// </summary>
	[InitializeOnLoad]
	public class SceneDirtyChecker
	{
		/// <summary>
		/// Whether the scene is dirty
		/// </summary>
		public static bool sceneIsDirty = false;

		/// <summary>
		/// Checker.
		/// </summary>
		static SceneDirtyChecker()
		{
			Undo.postprocessModifications += OnPostProcessModifications;
			Undo.undoRedoPerformed += OnUndoRedo;
		}

		/// <summary>
		/// Handles undo and redo events.
		/// </summary>
		static void OnUndoRedo()
		{
			string path = Main.GetProjectPath() + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
			Main.OnSceneChanged(path);
		}

		/// <summary>
		/// Handles modifications events.
		/// </summary>
		/// <param name="propertyModifications"></param>
		/// <returns></returns>
		static UndoPropertyModification[] OnPostProcessModifications(UndoPropertyModification[] propertyModifications)
		{
			sceneIsDirty = true;

			string path = Main.GetProjectPath() + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
			Main.OnSceneChanged(path);

			return propertyModifications;
		}
	}
}