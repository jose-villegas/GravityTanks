using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletLife : MonoBehaviour {
    public float timeToIgnoreOwner = 1f;

    void Start()
    {
        Invoke("ForgetOwner", timeToIgnoreOwner);
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
