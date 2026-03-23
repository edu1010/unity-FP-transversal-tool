using UnityEngine;

namespace TransversalExercises.Programming
{
    public class TransversalPlayerMovementSolution : MonoBehaviour
    {
        [SerializeField] float speed = 5f;

        void Update()
        {
            var h = Input.GetAxis("Horizontal");
            var v = Input.GetAxis("Vertical");
            var delta = new Vector3(h, 0f, v) * speed * Time.deltaTime;
            transform.Translate(delta, Space.World);
        }
    }
}
