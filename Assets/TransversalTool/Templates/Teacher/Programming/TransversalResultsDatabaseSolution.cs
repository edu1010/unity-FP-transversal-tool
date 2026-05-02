using UnityEngine;

namespace TransversalExercises.Programming
{
    [System.Serializable]
    public class MatchResultSolution
    {
        public string playerId;
        public int score;
        public string playedAt;
    }

    public class TransversalResultsDatabaseSolution : MonoBehaviour
    {
        MatchResultSolution lastResult;

        public void SaveResult(MatchResultSolution result) { lastResult = result; }

        public MatchResultSolution LoadLatestResult(string playerId)
        {
            if (lastResult == null || lastResult.playerId != playerId) return new MatchResultSolution { playerId = playerId, score = 0, playedAt = string.Empty };
            return lastResult;
        }
    }
}
