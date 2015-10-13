using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SphericalGravity : MonoBehaviour {

    public float gravitationalPull;
    public float pullRadius = 5f;
    public LayerMask pullFromMasks;


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }

    void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pullRadius, pullFromMasks);

        foreach(Collider other in colliders)
        {
            Rigidbody oRigidBody = other.GetComponent<Rigidbody>();

            if (oRigidBody != null)
            {
                Vector3 forceDirection = (transform.position - other.transform.position).normalized;
                oRigidBody.AddForce(forceDirection * gravitationalPull);
            }
        }
    }
}
