using UnityEngine;

namespace TransversalExercises.Programming
{
    [System.Serializable]
    public class EnemyData
    {
        public string enemyName;
        public int damage;
    }

    public class TransversalPlayerEnemyBasicsTemplate : MonoBehaviour
    {
        [SerializeField] string playerName = "Hero";
        [SerializeField] int playerHealth = 100;
        [SerializeField] EnemyData enemy = new EnemyData();

        void Start()
        {
            // TODO: Inicialitza les dades bàsiques del player i l'enemic.
            // TODO: Mostra per consola una simulació breu de l'atac de l'enemic.
        }
    }
}
