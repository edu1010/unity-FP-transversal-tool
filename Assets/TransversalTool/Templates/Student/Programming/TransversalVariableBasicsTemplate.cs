using UnityEngine;

namespace TransversalExercises.Programming
{
    public class TransversalVariableBasicsTemplate : MonoBehaviour
    {
        [Header("Canvia aquests valors des de l'Inspector")]
        [SerializeField] int lives = 3;
        [SerializeField] float speed = 4f;
        [SerializeField] bool invertHorizontal = false;

        void Update()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            // TODO: Si invertHorizontal és true, inverteix el valor horitzontal.
            // TODO: Mou el personatge amb speed.
            // TODO: Afegeix una prova amb tecla (per exemple, espai) per reduir lives en 1 i mostrar-lo per consola.
        }
    }
}
