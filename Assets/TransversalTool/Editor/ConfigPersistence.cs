using System.IO;
using UnityEditor;
using UnityEngine;

namespace TransversalTool
{
    public static class ConfigPersistence
    {
        public static void SaveConfig(GenerationConfig config, string path)
        {
            if (config == null || string.IsNullOrEmpty(path))
            {
                return;
            }

            var json = JsonUtility.ToJson(config, true);
            File.WriteAllText(path, json);
            AssetDatabase.Refresh();
        }

        public static GenerationConfig LoadConfig(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return null;
            }

            var json = File.ReadAllText(path);
            return JsonUtility.FromJson<GenerationConfig>(json);
        }

        public static string PromptSavePath(string defaultName)
        {
            return EditorUtility.SaveFilePanel("Save Config", Application.dataPath, defaultName, "json");
        }

        public static string PromptLoadPath()
        {
            return EditorUtility.OpenFilePanel("Load Config", Application.dataPath, "json");
        }
    }
}
