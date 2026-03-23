using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    [RequireComponent(typeof(TURTLE_BLACKBOARD))]
    public class FSM_TURTLE_Breathe : FiniteStateMachine
    {
        public enum State
        {
            INITIAL, REACH_SURFACE, TAKING_BREATH
        };

        public State currentState = State.INITIAL;
        private TURTLE_BLACKBOARD blackboard;


        void Awake()
        {
            blackboard = GetComponent<TURTLE_BLACKBOARD>();

            blackboard.definitiveBreathingPoint = blackboard.posibleBreathingPoints[Random.Range(0, blackboard.posibleBreathingPoints.Count)];

        }
        public override void Exit()
        {
            base.Exit();
        }

        public override void ReEnter()
        {
            currentState = State.INITIAL;
            blackboard.definitiveBreathingPoint = blackboard.posibleBreathingPoints[Random.Range(0, blackboard.posibleBreathingPoints.Count)];
            blackboard.changeToWander = false;
            base.ReEnter();
        }
        // Update is called once per frame
        void Update()
        {
            switch (currentState)
            {
                case State.INITIAL:
                    ChangeState(State.REACH_SURFACE);
                    break;
                case State.REACH_SURFACE:
                    if (Vector3.Distance(gameObject.transform.position, blackboard.definitiveBreathingPoint.transform.position) <= blackboard.bubbleReachedRadius)
                    {
                        ChangeState(State.TAKING_BREATH);
                        break;
                    }
                    break;
                case State.TAKING_BREATH:
                    if (blackboard.currentOxigen >= blackboard.maxOxigen)
                    {
                        blackboard.changeToWander = true;
                        break;
                    }
                    else 
                    {
                        blackboard.currentOxigen += Time.deltaTime*5;
                    }
                break;
            }
        }

        private void ChangeState(State newState)
        {
            // EXIT STATE LOGIC. Depends on current state
            switch (currentState)
            {
                case State.REACH_SURFACE:
                    break;
                case State.TAKING_BREATH:
                    blackboard.currentOxigen = blackboard.maxOxigen;
                    blackboard.definitiveBreathingPoint = null;
                    break;
            }

            // ENTER STATE LOGIC. Depends on newState
            switch (newState)
            {
                case State.REACH_SURFACE:
                  
                    break;
                case State.TAKING_BREATH:                    
                    break;
            }
            currentState = newState;
        }
    }
}

