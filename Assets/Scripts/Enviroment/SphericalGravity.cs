﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlanetInfo))]
public class SphericalGravity : MonoBehaviour
{
    public PlanetInfo PlanetInformation;
    public float PullRadius = 5f;
    public LayerMask PullMasks;


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, PullRadius);
    }

    void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, PullRadius, PullMasks);

        foreach(Collider other in colliders)
        {
            Rigidbody oRigidBody = other.GetComponent<Rigidbody>();

            if (oRigidBody == null) continue;

            Vector3 forceDirection = (transform.position - other.transform.position).normalized;
            oRigidBody.AddForce(forceDirection * PlanetInformation.Gravity);
        }
    }
}
