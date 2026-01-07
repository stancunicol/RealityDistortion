using UnityEngine;
using System.IO;

public class CameraScreenshot : MonoBehaviour
{
    public KeyCode screenshotKey = KeyCode.F12;

    private int width = 2560;  // 2K width
    private int height = 1440; // 2K height
    private int counter = 0;

    private void Update()
    {
        if (Input.GetKeyDown(screenshotKey))
        {
            TakeScreenshot();
        }
    }

    private void TakeScreenshot()
    {
        // Creează un RenderTexture 2K
        RenderTexture rt = new RenderTexture(width, height, 24);
        Camera cam = GetComponent<Camera>();
        cam.targetTexture = rt;

        // Renderiza camera în RenderTexture
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        cam.Render();

        // Copiază pixelii
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        // Curățare temporare
        cam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // Salvează screenshot
        string folder = Application.dataPath + "/Screenshots/";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string path = folder + "screenshot_" + counter + ".png";
        counter++;

        File.WriteAllBytes(path, screenshot.EncodeToPNG());
        Debug.Log("Screenshot 2K salvat: " + path);
    }
}
