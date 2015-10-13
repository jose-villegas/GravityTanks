using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {

    public Transform player;
    public Transform gravityPuller;
    public float velocityImpulse = 5f;
    public float detectionRange = 5f;
    public float outRangeSlowdown = 1f;
    public float limitCapsuleEnd = 0.95f;

    Rigidbody eRigidbody;
    public bool isPlayerInRange = false;
    int playerLayer;

    void Awake()
    {
        eRigidbody = GetComponent<Rigidbody>();
        playerLayer = LayerMask.GetMask("PlayerLayer");
    }

    void Start()
    {
        InvokeRepeating("DetectPlayer", 0.0f, 0.5f);
    }

    void FixedUpdate()
    {
        if (isPlayerInRange)
            eRigidbody.velocity = (player.position - transform.position).normalized * velocityImpulse * Time.fixedDeltaTime;
        else
            eRigidbody.velocity = Vector3.Lerp(eRigidbody.velocity, Vector3.zero, Time.fixedDeltaTime * outRangeSlowdown); // slowly slow down
    }

    void DetectPlayer()
    {
        float gPullerRadius = Mathf.Max(gravityPuller.lossyScale.x, gravityPuller.lossyScale.y, gravityPuller.lossyScale.z) / 2f;
        float targetDistance = Mathf.Sqrt(gPullerRadius * gPullerRadius - detectionRange * detectionRange);

        Vector3 circleNormal = (transform.position - gravityPuller.position).normalized;
        Vector3 capsuleOrigin = gravityPuller.position - circleNormal * (gPullerRadius * limitCapsuleEnd);
        Vector3 capsuleEnd = gravityPuller.position + circleNormal * (targetDistance + detectionRange);

        isPlayerInRange = Physics.CheckCapsule(capsuleOrigin, capsuleEnd, detectionRange, playerLayer);
    }

    void OnDrawGizmosSelected()
    {
        // gravity opposite direction
        if (eRigidbody != null) { 
            Gizmos.color = Color.cyan;
            DrawArrow.ForGizmo(transform.position, eRigidbody.velocity);
        }

        // bomb player detection range
        float gPullerRadius = Mathf.Max(gravityPuller.lossyScale.x, gravityPuller.lossyScale.y, gravityPuller.lossyScale.z) / 2f;
        float targetDistance = Mathf.Sqrt(gPullerRadius * gPullerRadius - detectionRange * detectionRange);

        Vector3 circleNormal = (transform.position - gravityPuller.position).normalized;
        Vector3 capsuleOrigin = gravityPuller.position - circleNormal * (gPullerRadius * limitCapsuleEnd);
        Vector3 capsuleEnd = gravityPuller.position + circleNormal * (targetDistance + detectionRange);

        DebugExtension.DrawCapsule(capsuleOrigin, capsuleEnd, Color.red, detectionRange);

        Matrix4x4 cubeSpace = new Matrix4x4();
        Vector3 planePosition = gravityPuller.position + circleNormal * targetDistance;
        cubeSpace.SetTRS(planePosition, Quaternion.LookRotation(circleNormal, Vector3.up), Vector3.one);

        DebugExtension.DrawLocalCube(cubeSpace, new Vector3(detectionRange + gPullerRadius, detectionRange + gPullerRadius, 0), Color.red);
    }
}
