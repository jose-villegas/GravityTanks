using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {

    public GameObject oPlayer;
    public float speed = 5f;

    Rigidbody eRigidbody;

    void Awake()
    {
        eRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        eRigidbody.velocity += (oPlayer.transform.position - transform.position).normalized * speed * Time.deltaTime;
    }

    void OnDrawGizmosSelected()
    {
        // gravity opposite direction
        Gizmos.color = Color.cyan;
        DrawArrow.ForGizmo(transform.position, eRigidbody.velocity);
    }
}
