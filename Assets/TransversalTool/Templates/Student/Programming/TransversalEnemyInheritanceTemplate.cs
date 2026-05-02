using UnityEngine;

namespace TransversalExercises.Programming
{
    public abstract class EnemyBaseTemplate
    {
        public string Name { get; }
        protected EnemyBaseTemplate(string name) { Name = name; }
        public abstract int Attack();
    }

    public class GroundEnemyTemplate : EnemyBaseTemplate
    {
        public GroundEnemyTemplate(string name) : base(name) { }
        public override int Attack() { return 0; }
    }

    public class FlyingEnemyTemplate : EnemyBaseTemplate
    {
        public FlyingEnemyTemplate(string name) : base(name) { }
        public override int Attack() { return 0; }
    }

    public class TransversalEnemyInheritanceTemplate : MonoBehaviour
    {
        void Start()
        {
            // TODO: Instanciar enemics i demostrar comportament polimòrfic.
        }
    }
}
