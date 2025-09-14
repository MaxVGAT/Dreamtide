using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class SceneShortcut
{
    [MenuItem("Scenes/Main Menu %#1")] // Ctrl+Shift+1
    private static void OpenMainMenu()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MainMenu.unity");
    }

    [MenuItem("Scenes/Intro Scene %#2")] // Ctrl+Shift+2
    private static void OpenIntroScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/IntroScene.unity");
    }

    [MenuItem("Scenes/Village Scene %#3")] // Ctrl+Shift+3
    private static void OpenVillageScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/VillageScene.unity");
    }
}
