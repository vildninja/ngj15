//Generated by SceneFind.cs 
using UnityEditor; 
namespace GameCore.UEditor { 
public class EditorMapOpener : Editor { 
	[UnityEditor.MenuItem("Open Scene/Logo", false, 20)]
	private static void Logo() {
		OpenIf("Assets/Scenes/Logo.unity");
	}
	[UnityEditor.MenuItem("Open Scene/Main Menu", false, 20)]
	private static void MainMenu() {
		OpenIf("Assets/Scenes/MainMenu.unity");
	}
	[UnityEditor.MenuItem("Open Scene/Arena_Fisk", false, 20)]
	private static void Arena_Fisk() {
		OpenIf("Assets/Arena_Fisk.unity");
	}
	[UnityEditor.MenuItem("Open Scene/Scene", false, 20)]
	private static void scene() {
		OpenIf("Assets/scene.unity");
	}
	[UnityEditor.MenuItem("Open Scene/Arena", false, 20)]
	private static void Arena() {
		OpenIf("Assets/Arena.unity");
	}
	[UnityEditor.MenuItem("Open Scene/Arena_Potholes", false, 20)]
	private static void Arena_Potholes() {
		OpenIf("Assets/Arena_Potholes.unity");
	}
	private static void OpenIf(string level) {
		if (EditorApplication.SaveCurrentSceneIfUserWantsTo()){
			EditorApplication.OpenScene(level);
		}
	}
}
}