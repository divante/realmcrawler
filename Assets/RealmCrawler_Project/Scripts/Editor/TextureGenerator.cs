using UnityEngine;
using UnityEditor;
using System.IO;

namespace RealmCrawler.Editor
{
    public class TextureGenerator : EditorWindow
    {
        private int textureSize = 512;
        private string textureName = "RadialGradient";
        private string savePath = "Assets/RealmCrawler_Project/Textures/VFX";

        [MenuItem("RealmCrawler/Tools/Texture Generator")]
        public static void ShowWindow()
        {
            GetWindow<TextureGenerator>("Texture Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Generate Mask Textures", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            textureSize = EditorGUILayout.IntSlider("Texture Size", textureSize, 128, 1024);
            textureName = EditorGUILayout.TextField("Texture Name", textureName);
            savePath = EditorGUILayout.TextField("Save Path", savePath);

            EditorGUILayout.Space();
            GUILayout.Label("Quick Generate:", EditorStyles.boldLabel);

            if (GUILayout.Button("Radial Gradient (Soft Circle)"))
            {
                GenerateRadialGradient("RadialGradient_Soft", 1.0f);
            }

            if (GUILayout.Button("Radial Gradient (Hard Edge)"))
            {
                GenerateRadialGradient("RadialGradient_Hard", 2.0f);
            }

            if (GUILayout.Button("Radial Gradient (Very Soft)"))
            {
                GenerateRadialGradient("RadialGradient_VerySoft", 0.5f);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Vertical Gradient"))
            {
                GenerateVerticalGradient("VerticalGradient");
            }

            if (GUILayout.Button("Horizontal Gradient"))
            {
                GenerateHorizontalGradient("HorizontalGradient");
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Perlin Noise"))
            {
                GeneratePerlinNoise("PerlinNoise", 4.0f);
            }

            if (GUILayout.Button("Perlin Noise (Fine)"))
            {
                GeneratePerlinNoise("PerlinNoise_Fine", 8.0f);
            }
        }

        private void GenerateRadialGradient(string name, float falloff)
        {
            Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
            Vector2 center = new Vector2(textureSize * 0.5f, textureSize * 0.5f);
            float maxDistance = textureSize * 0.5f;

            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    Vector2 pos = new Vector2(x, y);
                    float distance = Vector2.Distance(pos, center);
                    float normalizedDistance = distance / maxDistance;
                    float value = Mathf.Pow(1.0f - Mathf.Clamp01(normalizedDistance), falloff);

                    texture.SetPixel(x, y, new Color(value, value, value, value));
                }
            }

            texture.Apply();
            SaveTexture(texture, name);
        }

        private void GenerateVerticalGradient(string name)
        {
            Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);

            for (int y = 0; y < textureSize; y++)
            {
                float value = (float)y / textureSize;
                for (int x = 0; x < textureSize; x++)
                {
                    texture.SetPixel(x, y, new Color(value, value, value, value));
                }
            }

            texture.Apply();
            SaveTexture(texture, name);
        }

        private void GenerateHorizontalGradient(string name)
        {
            Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);

            for (int x = 0; x < textureSize; x++)
            {
                float value = (float)x / textureSize;
                for (int y = 0; y < textureSize; y++)
                {
                    texture.SetPixel(x, y, new Color(value, value, value, value));
                }
            }

            texture.Apply();
            SaveTexture(texture, name);
        }

        private void GeneratePerlinNoise(string name, float scale)
        {
            Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);

            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    float xCoord = (float)x / textureSize * scale;
                    float yCoord = (float)y / textureSize * scale;
                    float value = Mathf.PerlinNoise(xCoord, yCoord);

                    texture.SetPixel(x, y, new Color(value, value, value, value));
                }
            }

            texture.Apply();
            SaveTexture(texture, name);
        }

        private void SaveTexture(Texture2D texture, string name)
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            byte[] bytes = texture.EncodeToPNG();
            string fullPath = $"{savePath}/{name}.png";
            File.WriteAllBytes(fullPath, bytes);

            AssetDatabase.Refresh();

            TextureImporter importer = AssetImporter.GetAtPath(fullPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Default;
                importer.sRGBTexture = true;
                importer.alphaSource = TextureImporterAlphaSource.FromInput;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.filterMode = FilterMode.Bilinear;
                AssetDatabase.ImportAsset(fullPath, ImportAssetOptions.ForceUpdate);
            }

            Debug.Log($"Texture saved to: {fullPath}");
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Texture2D>(fullPath));
        }
    }
}
