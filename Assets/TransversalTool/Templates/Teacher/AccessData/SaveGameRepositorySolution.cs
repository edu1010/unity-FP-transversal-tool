using System;
using System.Data;
using UnityEngine;

namespace TransversalExercises.AccessData
{
    public class SaveGameRepositorySolution : MonoBehaviour
    {
        [SerializeField] string connectionString = "Data Source=savegame.db;";

        // This example uses an abstract connection factory to avoid provider lock-in.
        // In class, this can be replaced with SQLiteConnection or similar.
        public void SaveSnapshot(string saveGameJson, Func<IDbConnection> connectionFactory)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText =
                        "INSERT INTO savegame_snapshot (snapshot_json, created_at) VALUES (@json, @createdAt);";

                    var pJson = cmd.CreateParameter();
                    pJson.ParameterName = "@json";
                    pJson.Value = saveGameJson;
                    cmd.Parameters.Add(pJson);

                    var pCreatedAt = cmd.CreateParameter();
                    pCreatedAt.ParameterName = "@createdAt";
                    pCreatedAt.Value = DateTime.UtcNow.ToString("o");
                    cmd.Parameters.Add(pCreatedAt);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public string LoadLatestSnapshot(Func<IDbConnection> connectionFactory)
        {
            using (var connection = connectionFactory())
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText =
                        "SELECT snapshot_json FROM savegame_snapshot ORDER BY created_at DESC LIMIT 1;";
                    var result = cmd.ExecuteScalar();
                    return result == null || result == DBNull.Value ? string.Empty : result.ToString();
                }
            }
        }
    }
}
