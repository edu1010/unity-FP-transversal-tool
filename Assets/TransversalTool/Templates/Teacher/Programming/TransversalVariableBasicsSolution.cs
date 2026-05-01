using UnityEngine;

namespace TransversalExercises.Programming
{
    public class TransversalVariableBasicsSolution : MonoBehaviour
    {
        [Header("Canvia aquests valors des de l'Inspector")]
        [SerializeField] int lives = 3;
        [SerializeField] float speed = 4f;
        [SerializeField] bool invertHorizontal = false;

        void Update()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            if (invertHorizontal)
            {
                horizontal *= -1f;
            }

            var delta = new Vector3(horizontal, 0f, vertical) * speed * Time.deltaTime;
            transform.Translate(delta, Space.World);

            if (Input.GetKeyDown(KeyCode.Space) && lives > 0)
            {
                lives--;
                Debug.Log("Vides restants: " + lives);
            }
        }
    }
}
