using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TURTLE_BLACKBOARD : MonoBehaviour
{
    [Header("Wander")]
    public float maxTimeWandering = 15.0f;
    public float minTimeWandering = 6.0f;
    public float maxTimeHavingFun = 5.0f;
    public float maxTimeSayingIt = 3.0f;
    public GameObject Attractor;
    public GameObject textTurtle;

    [Header("Breathe")]
    public float oxigenToBreathe = 20.0f; //if the current oxigen arrives to this value, the turtle will go to breath
    public float currentOxigen;
    public float maxOxigen = 100.0f;    
    public float bubbleReachedRadius = 1.0f;
    public bool changeToWander = false;
    public List<GameObject> posibleBreathingPoints;
    public GameObject definitiveBreathingPoint;

    private void Start()
    {
        textTurtle = GameObject.Find("TextTurtle");
        textTurtle.SetActive(false);
    }

    public void SayIt(bool on)
    {
        textTurtle.SetActive(on);
    }
}
