using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public float Damage = 10;
    private void OnTriggerEnter2D(Collider2D collider)
    {
        var reciever = collider.GetComponent<IRecieveDamage>();
        if (reciever != null)
        {
            reciever.TakeDamage(Damage);
        }
    }
}
