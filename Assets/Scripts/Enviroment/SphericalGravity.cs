using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SphericalGravity : MonoBehaviour {

    public float gravitationalPull;

    LayerMask ignorePullMask;

    void FixedUpdate()
    {
        ignorePullMask = LayerMask.GetMask("IgnoreGPull");
    }

    void OnTriggerStay(Collider other)
    {
        // this object ignores planets gravity pull, useful for hard-wire satellites
        if ((1 << other.gameObject.layer & ignorePullMask) > 0) return;

        Rigidbody oRigidBody = other.GetComponent<Rigidbody>();

        if (oRigidBody != null)
        {
            Vector3 forceDirection = (transform.position - other.transform.position).normalized;
            oRigidBody.AddForce(forceDirection * gravitationalPull);
        }  
    }
}
