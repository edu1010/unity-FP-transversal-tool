using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FSM
{
    [RequireComponent(typeof(FSM_TURTLE_Wander))]
    [RequireComponent(typeof(FSM_TURTLE_Breathe))]
    [RequireComponent(typeof(TURTLE_BLACKBOARD))]

    public class FSM_TURTLE : FiniteStateMachine
    {
        public enum State
        {
            INITIAL, WANDER, BREATHE
        };

        public State currentState = State.INITIAL;
        private TURTLE_BLACKBOARD blackboard;
        private FSM_TURTLE_Wander turtleFsmWander;
        private FSM_TURTLE_Breathe turtleFsmBreathe;

        // Start is called before the first frame update
        void Start()
        {
            blackboard = GetComponent<TURTLE_BLACKBOARD>();
            turtleFsmWander = GetComponent<FSM_TURTLE_Wander>();
            turtleFsmBreathe = GetComponent<FSM_TURTLE_Breathe>();

            turtleFsmWander.enabled = false;
            turtleFsmBreathe.enabled = false;

            blackboard.currentOxigen = blackboard.maxOxigen;
        }
        public override void Exit()
        {
            turtleFsmWander.enabled = false;
            turtleFsmBreathe.enabled = false;
            base.Exit();
        }

        public override void ReEnter()
        {
            currentState = State.INITIAL;
            base.ReEnter();
        }
        // Update is called once per frame
        void Update()
        {
            switch (currentState)
            {
                case State.INITIAL:
                    ChangeState(State.WANDER);
                    break;
                case State.WANDER:                    
                    if(blackboard.currentOxigen <= blackboard.oxigenToBreathe)
                    {
                        ChangeState(State.BREATHE);
                        break;
                    }
                    blackboard.currentOxigen -= 2 * Time.deltaTime;
                    break;
                case State.BREATHE:
                    if (blackboard.currentOxigen >= blackboard.maxOxigen)
                    {
                        blackboard.currentOxigen = blackboard.maxOxigen;
                        ChangeState(State.WANDER);
                        break;
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
                    turtleFsmWander.Exit();
                    break;
                case State.BREATHE:
                    turtleFsmBreathe.Exit();
                    break;
            }

            // ENTER STATE LOGIC. Depends on newState
            switch (newState)
            {
                case State.WANDER:
                    turtleFsmWander.ReEnter();
                    break;
                case State.BREATHE:
                    turtleFsmBreathe.ReEnter();
                    break;
            }
            currentState = newState;
        }
    }
}
