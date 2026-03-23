using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnHit : MonoBehaviour
{
    private HealthSystem health;
    public void Awake()
    {
        health = GetComponent<HealthSystem>();
        
    }
    private void OnEnable()
    {
        health.OnDeath += OnDeath;
    }
    private void OnDisable()
    {
        health.OnDeath -= OnDeath;
    }
    private void OnDeath()
    {
        SceneLoader.Instance.LoadLevel(2); // game over scene
    }
}
