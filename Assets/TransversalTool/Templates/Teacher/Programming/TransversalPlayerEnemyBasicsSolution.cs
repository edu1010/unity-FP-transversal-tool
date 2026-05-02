using UnityEngine;

namespace TransversalExercises.Programming
{
    [System.Serializable]
    public class EnemyDataSolution
    {
        public string enemyName;
        public int damage;
    }

    public class TransversalPlayerEnemyBasicsSolution : MonoBehaviour
    {
        [SerializeField] string playerName = "Hero";
        [SerializeField] int playerHealth = 100;
        [SerializeField] EnemyDataSolution enemy = new EnemyDataSolution { enemyName = "Slime", damage = 15 };

        void Start()
        {
            var remainingHealth = Mathf.Max(0, playerHealth - enemy.damage);
            Debug.Log($"{enemy.enemyName} ataca {playerName}. Vida restant: {remainingHealth}");
        }
    }
}
