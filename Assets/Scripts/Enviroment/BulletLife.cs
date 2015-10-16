using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletLife : MonoBehaviour
{
    /// <summary>
    /// Bullets have an initial owner to avoid self collision
    /// </summary>
    public float TimeToIgnoreOwner = 1f;

    void Start()
    {
        Invoke("ForgetOwner", TimeToIgnoreOwner);
    }

    void ForgetOwner()
    {
        gameObject.tag = "Untagged";
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == gameObject.tag) return;

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        DrawArrow.ForGizmo(transform.position, GetComponent<Rigidbody>().velocity);
    }
}
