using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class CollisionEvent : UnityEvent<Transform> { }
public class KickCollider : MonoBehaviour
{

    public CollisionEvent Hit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Hit.Invoke(other.transform);
        }
    }
}
