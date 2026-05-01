using UnityEngine;

namespace TransversalExercises.Programming
{
    public class TransversalCoinPickupGiven : MonoBehaviour
    {
        [SerializeField] int coinValue = 1;

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            Debug.Log("Moneda recollida: +" + coinValue);
            Destroy(gameObject);
        }
    }
}
