﻿using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
    public Transform target;
    public float heightToTarget = 10f;
    public float smoothing = 5f;

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 targetCamPos = target.position + target.up * heightToTarget;
        Vector3 targetDirection = target.position - transform.position;
        transform.position = Vector3.Slerp(transform.position, targetCamPos, smoothing * Time.deltaTime);

        Quaternion lookToPlayer = Quaternion.LookRotation(targetDirection, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookToPlayer, smoothing * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawLine(transform.position, target.position);
    }
}
