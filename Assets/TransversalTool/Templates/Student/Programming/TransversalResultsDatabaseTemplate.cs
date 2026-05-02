using UnityEngine;

namespace TransversalExercises.Programming
{
    [System.Serializable]
    public class MatchResultTemplate
    {
        public string playerId;
        public int score;
        public string playedAt;
    }

    public class TransversalResultsDatabaseTemplate : MonoBehaviour
    {
        public void SaveResult(MatchResultTemplate result)
        {
            // TODO: Persistir resultat a capa de dades.
        }

        public MatchResultTemplate LoadLatestResult(string playerId)
        {
            // TODO: Recuperar últim resultat del jugador.
            return new MatchResultTemplate { playerId = playerId };
        }
    }
}
