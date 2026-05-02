using UnityEngine;

namespace TransversalExercises.Programming
{
    public abstract class EnemyBaseSolution
    {
        public string Name { get; }
        protected EnemyBaseSolution(string name) { Name = name; }
        public abstract int Attack();
    }

    public class GroundEnemySolution : EnemyBaseSolution
    {
        public GroundEnemySolution(string name) : base(name) { }
        public override int Attack() => 12;
    }

    public class FlyingEnemySolution : EnemyBaseSolution
    {
        public FlyingEnemySolution(string name) : base(name) { }
        public override int Attack() => 18;
    }

    public class TransversalEnemyInheritanceSolution : MonoBehaviour
    {
        void Start()
        {
            EnemyBaseSolution[] enemies = { new GroundEnemySolution("Crawler"), new FlyingEnemySolution("Bat") };
            foreach (var enemy in enemies) Debug.Log($"{enemy.Name} fa {enemy.Attack()} de dany");
        }
    }
}
