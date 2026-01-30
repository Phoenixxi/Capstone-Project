using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureRotator : Editor
{
    // Adds a menu item to the right-click context menu in the Project window
    [MenuItem("Assets/VFX/Spin Texture 90бу (Create Copy)", false, 10)]
    private static void RotateTexture90()
    {
        Object selectedObject = Selection.activeObject;

        // 1. Validate Selection
        if (selectedObject == null || !(selectedObject is Texture2D))
        {
            Debug.LogWarning("Please select a Texture2D asset.");
            return;
        }

        string path = AssetDatabase.GetAssetPath(selectedObject);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer == null) return;

        // 2. Ensure the texture is readable so we can access pixels
        bool wasReadable = importer.isReadable;
        TextureImporterCompression originalCompression = importer.textureCompression;

        if (!wasReadable || originalCompression != TextureImporterCompression.Uncompressed)
        {
            importer.isReadable = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        Texture2D sourceTex = (Texture2D)selectedObject;
        int srcW = sourceTex.width;
        int srcH = sourceTex.height;

        // 3. Create new texture container (Swapping Width and Height)
        Texture2D newTex = new Texture2D(srcH, srcW, sourceTex.format, sourceTex.mipmapCount > 1);

        // 4. Rotate Pixels (Clockwise 90)
        Color[] sourcePixels = sourceTex.GetPixels();
        Color[] rotatedPixels = new Color[sourcePixels.Length];

        for (int y = 0; y < srcH; y++)
        {
            for (int x = 0; x < srcW; x++)
            {
                // Source Index (Row-major)
                int srcIdx = y * srcW + x;

                // Rotate Logic:
                // New X = Old Y
                // New Y = (Old Width - 1) - Old X
                int destX = y;
                int destY = (srcW - 1) - x;

                // Destination Index (Use srcH as the new stride/width)
                int destIdx = destY * srcH + destX;

                rotatedPixels[destIdx] = sourcePixels[srcIdx];
            }
        }

        newTex.SetPixels(rotatedPixels);
        newTex.Apply();

        // 5. Restore original import settings
        if (!wasReadable || originalCompression != TextureImporterCompression.Uncompressed)
        {
            importer.isReadable = wasReadable;
            importer.textureCompression = originalCompression;
            importer.SaveAndReimport();
        }

        // 6. Save as a new file
        byte[] bytes = newTex.EncodeToPNG();
        string dir = Path.GetDirectoryName(path);
        string fileName = Path.GetFileNameWithoutExtension(path);
        string newPath = Path.Combine(dir, $"{fileName}_Rotated90.png");

        // Ensure we don't overwrite if one already exists
        newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);

        File.WriteAllBytes(newPath, bytes);
        AssetDatabase.Refresh();

        // 7. Cleanup and Select
        Object.DestroyImmediate(newTex);

        Object newAsset = AssetDatabase.LoadAssetAtPath<Object>(newPath);
        Selection.activeObject = newAsset;

        Debug.Log($"<color=#44ffaa><b>[VFX Helper]</b></color> Created rotated copy: {newPath}");
    }

    // Validation to grey out the menu item if not a texture
    [MenuItem("Assets/VFX/Spin Texture 90бу (Create Copy)", true)]
    private static bool ValidateRotateTexture()
    {
        return Selection.activeObject is Texture2D;
    }
}