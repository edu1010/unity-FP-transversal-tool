using UnityEngine;

namespace TransversalExercises.Programming
{
    public class TransversalCoinSpawnerSolution : MonoBehaviour
    {
        [SerializeField] GameObject coinPrefab;
        [SerializeField] int coinCount = 5;
        [SerializeField] float spacing = 1.5f;

        void Start()
        {
            if (coinPrefab == null)
            {
                Debug.LogWarning("Coin prefab no assignat.");
                return;
            }

            for (var i = 0; i < coinCount; i++)
            {
                var spawnPos = transform.position + new Vector3(i * spacing, 0f, 0f);
                Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            }
        }
    }
}
