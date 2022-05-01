using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenceTools : MonoBehaviour
{
    public static Scene nowScene;

    [MenuItem("SceneTool/跳转到Loading场景")]
    static void GoLoadingScene()
    {
        nowScene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.SaveScene(nowScene);
        EditorSceneManager.OpenScene("Assets/Levels/Loading.unity");
    }

    [MenuItem("SceneTool/跳转到CharSelect场景")]
    static void GoCharSelectScene()
    {
        nowScene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.SaveScene(nowScene);
        EditorSceneManager.OpenScene("Assets/Levels/CharSelect.unity");
    }

    [MenuItem("SceneTool/跳转到MainCity场景")]
    static void GoMainScene()
    {
        nowScene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.SaveScene(nowScene);
        EditorSceneManager.OpenScene("Assets/Levels/MainCity.unity");
    }

    [MenuItem("SceneTool/跳转到Map01场景")]
    static void GoMap01Scene()
    {
        nowScene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.SaveScene(nowScene);
        EditorSceneManager.OpenScene("Assets/Levels/Map01.unity");
    }

    [MenuItem("SceneTool/跳转到Map02场景")]
    static void GoMap02Scene()
    {
        nowScene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.SaveScene(nowScene);
        EditorSceneManager.OpenScene("Assets/Levels/Map02.unity");
    }

    [MenuItem("SceneTool/跳转到Map03场景")]
    static void GoMap03Scene()
    {
        nowScene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.SaveScene(nowScene);
        EditorSceneManager.OpenScene("Assets/Levels/Map03.unity");
    }

}
