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
	[UnityEditor.MenuItem("Open Scene/Arena", false, 20)]
	private static void Arena() {
		OpenIf("Assets/Arena.unity");
	}
	private static void OpenIf(string level) {
		if (EditorApplication.SaveCurrentSceneIfUserWantsTo()){
			EditorApplication.OpenScene(level);
		}
	}
}
}