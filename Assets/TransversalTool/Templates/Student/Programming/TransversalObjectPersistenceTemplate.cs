using UnityEngine;

namespace TransversalExercises.Programming
{
    [System.Serializable]
    public class PlayerProfileTemplate
    {
        public string playerId;
        public int level;
        public int score;
    }

    public class TransversalObjectPersistenceTemplate : MonoBehaviour
    {
        public string SerializeProfile(PlayerProfileTemplate profile)
        {
            // TODO: Serialitzar profile a JSON.
            return string.Empty;
        }

        public PlayerProfileTemplate DeserializeProfile(string raw)
        {
            // TODO: Desserialitzar JSON a objecte PlayerProfileTemplate.
            return new PlayerProfileTemplate();
        }
    }
}
