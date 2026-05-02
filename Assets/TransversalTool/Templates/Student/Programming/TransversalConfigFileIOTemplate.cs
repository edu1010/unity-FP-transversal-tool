using System.IO;
using UnityEngine;

namespace TransversalExercises.Programming
{
    [System.Serializable]
    public class GameConfigTemplate
    {
        public string playerName;
        public int musicVolume;
        public bool subtitlesEnabled;
    }

    public class TransversalConfigFileIOTemplate : MonoBehaviour
    {
        const string FileName = "game_config.json";

        public void SaveConfig(GameConfigTemplate config)
        {
            // TODO: Serialitzar i guardar config a Application.persistentDataPath.
        }

        public GameConfigTemplate LoadConfig()
        {
            // TODO: Carregar i desserialitzar la configuració.
            return new GameConfigTemplate();
        }

        string GetPath() => Path.Combine(Application.persistentDataPath, FileName);
    }
}
