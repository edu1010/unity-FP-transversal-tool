using UnityEngine;

namespace TransversalExercises.Programming
{
    public class TransversalCoinSpawnerTemplate : MonoBehaviour
    {
        [SerializeField] GameObject coinPrefab;
        [SerializeField] int coinCount = 5;
        [SerializeField] float spacing = 1.5f;

        void Start()
        {
            // TODO: Fes un bucle for per instanciar coinCount monedes.
            // TODO: Col·loca cada moneda separada per "spacing" a l'eix X.
            // Pista: transform.position + new Vector3(i * spacing, 0f, 0f)
        }
    }
}
