using System.IO;
using UnityEngine;

namespace TransversalExercises.Programming
{
    [System.Serializable]
    public class GameConfigSolution
    {
        public string playerName;
        public int musicVolume;
        public bool subtitlesEnabled;
    }

    public class TransversalConfigFileIOSolution : MonoBehaviour
    {
        const string FileName = "game_config.json";

        public void SaveConfig(GameConfigSolution config)
        {
            File.WriteAllText(GetPath(), JsonUtility.ToJson(config, true));
        }

        public GameConfigSolution LoadConfig()
        {
            var path = GetPath();
            if (!File.Exists(path)) return new GameConfigSolution { playerName = "Player", musicVolume = 70, subtitlesEnabled = true };
            return JsonUtility.FromJson<GameConfigSolution>(File.ReadAllText(path));
        }

        string GetPath() => Path.Combine(Application.persistentDataPath, FileName);
    }
}
