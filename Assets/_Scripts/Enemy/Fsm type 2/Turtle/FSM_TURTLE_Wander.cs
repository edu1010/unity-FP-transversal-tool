using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FSM
{
    [RequireComponent(typeof(TURTLE_BLACKBOARD))]
    public class FSM_TURTLE_Wander : FiniteStateMachine
    {

        public enum State
        {
            INITIAL,
            WANDER,
            HAVING_FUN,
            SAY_IT,
        };

        public State currentState = State.INITIAL;

        private TURTLE_BLACKBOARD blackboard;
        
        private float elapsedTime = 0.0f;
        private float wanderingTime = 0.0f;

        void Awake()
        {
            blackboard = GetComponent<TURTLE_BLACKBOARD>();

        }

        public override void Exit()
        {
            blackboard.SayIt(false);
            base.Exit();
        }

        public override void ReEnter()
        {
            currentState = State.INITIAL;
            base.ReEnter();
        }

        void Update()
        {
            switch (currentState)
            {
                case State.INITIAL:
                    ChangeState(State.WANDER);
                    break;
                case State.WANDER:
                    elapsedTime += Time.deltaTime;
                    if(elapsedTime >= wanderingTime)
                    {
                        ChangeState(State.HAVING_FUN); break;
                    }
                    break;
                case State.HAVING_FUN:
                    transform.Rotate(new Vector3(0.0f, 0.0f, 5.0f));
                    elapsedTime += Time.deltaTime;
                    if(elapsedTime >= blackboard.maxTimeHavingFun)
                    {
                        ChangeState(State.SAY_IT); break;
                    }
                    break;
                case State.SAY_IT:
                    elapsedTime += Time.deltaTime;
                    if(elapsedTime >= blackboard.maxTimeSayingIt)
                    {
                        ChangeState(State.WANDER); break;
                    }
                    break;
            }
        }

        private void ChangeState(State newState)
        {
            // EXIT STATE LOGIC. Depends on current state
            switch (currentState)
            {
                case State.WANDER:
                    elapsedTime = 0.0f;
                    break;
                case State.HAVING_FUN:
                    elapsedTime = 0.0f;
                    break;
                case State.SAY_IT:
                    elapsedTime = 0.0f;
                    blackboard.SayIt(false);
                    break;
            }

            // ENTER STATE LOGIC. Depends on newState
            switch (newState)
            {
                case State.WANDER:
                    wanderingTime = Random.Range(blackboard.minTimeWandering, blackboard.maxTimeWandering);
                    
                    break;
                case State.HAVING_FUN:

                    break;
                case State.SAY_IT:
                    blackboard.SayIt(true);
                    break;
            }
            currentState = newState;
        }
    }
}