using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ScenceTools : MonoBehaviour
{
    [MenuItem("SceneTool/跳转到Loading场景")]
    static void GoUIScene()
    {
        EditorSceneManager.OpenScene("Assets/Levels/Loading.unity");
    }


    [MenuItem("SceneTool/跳转到MainCity场景")]
    static void GoMainScene()
    {
        EditorSceneManager.OpenScene("Assets/Levels/MainCity.unity");
    }

    [MenuItem("SceneTool/跳转到Map01场景")]
    static void GoMap01Scene()
    {
        EditorSceneManager.OpenScene("Assets/Levels/Map01.unity");
    }

    [MenuItem("SceneTool/跳转到Map02场景")]
    static void GoMap02Scene()
    {
        EditorSceneManager.OpenScene("Assets/Levels/Map02.unity");
    }

    [MenuItem("SceneTool/跳转到Map03场景")]
    static void GoMap03Scene()
    {
        EditorSceneManager.OpenScene("Assets/Levels/Map03.unity");
    }

}
