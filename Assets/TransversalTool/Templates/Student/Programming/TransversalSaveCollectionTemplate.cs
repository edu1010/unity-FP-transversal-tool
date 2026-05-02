using System;
using System.Collections.Generic;
using UnityEngine;

namespace TransversalExercises.Programming
{
    [System.Serializable]
    public class SaveEntryTemplate
    {
        public string id;
        public int score;
        public string savedAt;
    }

    public class TransversalSaveCollectionTemplate : MonoBehaviour
    {
        [SerializeField] List<SaveEntryTemplate> saves = new List<SaveEntryTemplate>();

        public List<SaveEntryTemplate> GetTopScores(int limit)
        {
            // TODO: Retornar els N millors scores ordenats descendent.
            return new List<SaveEntryTemplate>();
        }

        public Dictionary<string, SaveEntryTemplate> BuildIndexById()
        {
            // TODO: Construir diccionari per id de partida.
            return new Dictionary<string, SaveEntryTemplate>();
        }
    }
}
