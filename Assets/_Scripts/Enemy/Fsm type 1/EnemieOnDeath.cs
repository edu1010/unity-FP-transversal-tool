using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemieOnDeath : MonoBehaviour
{
    public delegate void RemoveTarget(Transform transform);
    public static RemoveTarget OnRemoveTarget;
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
        OnRemoveTarget?.Invoke(transform);
        Destroy(gameObject);
    }
}
