using UnityEngine;
using UnityEditor;
using System.IO;

public class EditorScreenshot
{
    [MenuItem("Tools/Prendre une capture %#p")]
    static void TakeScreenshot()
    {
        // Récupère la Scene View
        SceneView sceneView = SceneView.lastActiveSceneView;

        if (sceneView == null)
        {
            Debug.LogError("Aucune SceneView active !");
            return;
        }

        Camera cam = sceneView.camera;

        int width = 1920;
        int height = 1080;

        RenderTexture rt = new RenderTexture(width, height, 24);
        cam.targetTexture = rt;

        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        cam.Render();

        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        cam.targetTexture = null;
        RenderTexture.active = null;

        byte[] bytes = screenshot.EncodeToPNG();

        string path = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "SceneScreenshot.png");
        File.WriteAllBytes(path, bytes);

        Debug.Log("Capture sauvegardée ici : " + path);
    }
}