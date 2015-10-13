using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
    public float movementSpeed = 5f;
    public float damping = 8f;
    public Transform gravityPuller;

    Vector3 movement;
    Vector3 turnDirection;
    Vector3 upToCenterG;
    Rigidbody playerRigidbody;
    bool isWalking = false;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        // flags
        isWalking = h != 0 || v != 0;
        // player movement calls
        Turn(h, v);
        Move(h, v);
    }

    void Move(float h, float v)
    {
        if (!isWalking) return;

        movement = transform.right * h + transform.forward * v;
        movement = movement.normalized * movementSpeed * Time.fixedDeltaTime;

        playerRigidbody.MovePosition(transform.position + movement);
    }

    void Turn(float h, float v)
    {
        upToCenterG = (transform.position - gravityPuller.position).normalized;
        playerRigidbody.transform.up = Vector3.Slerp(transform.up, upToCenterG, damping * Time.deltaTime);

        if (!isWalking) return;

        turnDirection = transform.right * h + transform.forward * v;
        Quaternion lookAtTurn = Quaternion.LookRotation(turnDirection, upToCenterG);
        playerRigidbody.MoveRotation(lookAtTurn);
    }

    void OnDrawGizmosSelected()
    {
        // velocity vector direction
        if (playerRigidbody != null && playerRigidbody.velocity.magnitude > 0)
        {
            Gizmos.color = Color.cyan;
            DrawArrow.ForGizmo(transform.position, playerRigidbody.velocity.normalized);
        }
        // draw initial velocity vector
        float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

        // turn direction
        Gizmos.color = Color.black;
        DrawArrow.ForGizmo(transform.position + turnDirection * maxScale, turnDirection);

        // gravity opposite direction
        Gizmos.color = Color.cyan;
        DrawArrow.ForGizmo(transform.position + upToCenterG * maxScale, upToCenterG);
    }
}
