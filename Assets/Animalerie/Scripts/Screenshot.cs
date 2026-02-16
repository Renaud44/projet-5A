using UnityEngine;
using UnityEditor;

public class Screenshot
{
    [MenuItem("Tools/Prendre une capture %#p")] 
    // Ctrl + Shift + P
    static void TakeScreenshot()
    {
        string path = "EditorScreenshot.png";
        ScreenCapture.CaptureScreenshot(path);
        Debug.Log("Capture sauvegardée : " + path);
    }
}
