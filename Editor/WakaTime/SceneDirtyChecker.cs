using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;

namespace WakaTime {

	[InitializeOnLoad]
	public class SceneDirtyChecker {

		public static bool sceneIsDirty = false;
	
		static SceneDirtyChecker () {
			Undo.postprocessModifications += OnPostProcessModifications;
			Undo.undoRedoPerformed += OnUndoRedo;
		}
	
		static void OnUndoRedo () {
			string path = Main.GetProjectPath () + EditorSceneManager.GetActiveScene().name;
			Main.OnSceneChanged (path);
		}
	
		static UndoPropertyModification[] OnPostProcessModifications (UndoPropertyModification[] propertyModifications) {
			sceneIsDirty = true;

			string path = Main.GetProjectPath () + EditorSceneManager.GetActiveScene().name;
			Main.OnSceneChanged (path);

			return propertyModifications;
		}
	}
}