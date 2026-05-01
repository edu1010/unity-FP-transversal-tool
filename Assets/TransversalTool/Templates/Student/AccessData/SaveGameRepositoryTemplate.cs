using System;
using System.Data;
using UnityEngine;

namespace TransversalExercises.AccessData
{
    public class SaveGameRepositoryTemplate : MonoBehaviour
    {
        // Aquesta factoría és un punt d'entrada simplificat per a la connexió.
        // En un projecte real s'injectaria un proveïdor de connexions.
        [SerializeField] string connectionString = "Data Source=savegame.db;";

        public void SaveSnapshot(string saveGameJson)
        {
            // TODO: Obrir connexió.
            // TODO: Insertar saveGameJson i marca temporal a la taula savegame_snapshot.
            // TODO: Utilitzar paràmetres en la consulta.
            Debug.Log("TODO SaveSnapshot -> " + saveGameJson);
        }

        public string LoadLatestSnapshot()
        {
            // TODO: Obrir connexió.
            // TODO: Recuperar l'últim snapshot segons created_at descendent.
            // TODO: Retornar el json (string) o cadena buida si no hi ha dades.
            return string.Empty;
        }
    }
}
