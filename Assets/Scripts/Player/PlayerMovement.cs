using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour {

    public float MovementSpeed = 5f;
    public float Damping = 8f;
    public float JumpStrength = 2f;
    public Transform GravityPuller;
    public CameraMovement GameCamera;

    Vector3 _movement;
    Vector3 _turnDirection = Vector3.up;
    Vector3 _upToCenterG = Vector3.forward;
    Rigidbody _playerRigidbody;

    bool _isWalking;
    

    void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float f = Input.GetAxis("Jump");

        // flags
        _isWalking = Mathf.Abs(h) > Mathf.Epsilon || Mathf.Abs(v) > Mathf.Epsilon;
        // player movement calls
        Turn(h, v);
        Move(h, v);
        Jump(f);
    }

    void Move(float h, float v)
    {
        if (!_isWalking) return;

        _movement = GameCamera.NonInterpolatedTransform.right * h + GameCamera.NonInterpolatedTransform.up * v;
        _movement = _movement.normalized * MovementSpeed * Time.fixedDeltaTime;

        _playerRigidbody.MovePosition(transform.position + _movement);
    }

    void Turn(float h, float v)
    {
        _upToCenterG = (transform.position - GravityPuller.position).normalized;

        // smooth rotation with damping parameter
        if (_isWalking)
        {
            Transform mainCamera = Camera.main.transform;

            _turnDirection = Quaternion.LookRotation(mainCamera.up, _upToCenterG) * new Vector3(h, 0f, v);
            Quaternion rotAround = Quaternion.LookRotation(_turnDirection, _upToCenterG);
            // look forward to input plane direction
            _playerRigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, rotAround, Damping * Time.deltaTime));
        }
        else
        {
            Quaternion rotAround = Quaternion.LookRotation(_turnDirection, _upToCenterG);
            // rot to up vector based on gravity
            _playerRigidbody.MoveRotation(Quaternion.Slerp(transform.rotation, rotAround, Damping * Time.deltaTime));
        }
    }

    void Jump(float f)
    {
        if (f <= 0f) return;

        _playerRigidbody.velocity = transform.up * f * JumpStrength;
    }

    void OnDrawGizmosSelected()
    {
        if(Application.isPlaying)
        {
            // velocity vector direction
            Gizmos.color = Color.magenta;
            DrawArrow.ForGizmo(transform.position, _playerRigidbody.velocity);
        }
        
        // draw initial velocity vector
        float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

        // gravity opposite direction
        Gizmos.color = Color.cyan;
        DrawArrow.ForGizmo(transform.position + _upToCenterG * maxScale, _upToCenterG);

        // axis input plane
        Gizmos.color = Color.red;
        DebugExtension.DrawCircle(transform.position, _upToCenterG, Color.red, 1f);
        // draw current input position
        Gizmos.DrawCube(transform.position + _turnDirection, Vector3.one * 0.15f);
    }
}
