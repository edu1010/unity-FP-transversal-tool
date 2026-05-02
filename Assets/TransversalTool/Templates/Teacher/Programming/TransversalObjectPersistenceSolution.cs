using UnityEngine;

namespace TransversalExercises.Programming
{
    [System.Serializable]
    public class PlayerProfileSolution
    {
        public string playerId;
        public int level;
        public int score;
    }

    public class TransversalObjectPersistenceSolution : MonoBehaviour
    {
        public string SerializeProfile(PlayerProfileSolution profile) => JsonUtility.ToJson(profile, true);

        public PlayerProfileSolution DeserializeProfile(string raw)
        {
            return string.IsNullOrWhiteSpace(raw) ? new PlayerProfileSolution() : JsonUtility.FromJson<PlayerProfileSolution>(raw);
        }
    }
}
