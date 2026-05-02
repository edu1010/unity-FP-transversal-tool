using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TransversalExercises.Programming
{
    [System.Serializable]
    public class SaveEntrySolution
    {
        public string id;
        public int score;
        public string savedAt;
    }

    public class TransversalSaveCollectionSolution : MonoBehaviour
    {
        [SerializeField] List<SaveEntrySolution> saves = new List<SaveEntrySolution>();

        public List<SaveEntrySolution> GetTopScores(int limit)
        {
            return saves.OrderByDescending(s => s.score).Take(limit).ToList();
        }

        public Dictionary<string, SaveEntrySolution> BuildIndexById()
        {
            var map = new Dictionary<string, SaveEntrySolution>();
            foreach (var save in saves) map[save.id] = save;
            return map;
        }
    }
}
