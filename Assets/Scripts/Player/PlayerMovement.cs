using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(StickToPlanet))]
public class PlayerMovement : MonoBehaviour
{
    public float MovementSpeed = 5f;
    public float JumpStrength = 2f;
    /// <summary>
    /// Torque based solution to kill angular velocity
    /// this value decides how fast this happens
    /// </summary>
    public float KillAngularVelocity = 0.1f;
    public float Damping = 8f;
    public CameraMovement GameCamera;

    private Vector3 _movement;
    private Vector3 _turnDirection = Vector3.up;
    private Rigidbody _pRigidbody;
    private StickToPlanet _stickToPlanet;

    bool _isWalking;


    void Awake()
    {
        _pRigidbody = GetComponent<Rigidbody>();
        _stickToPlanet = GetComponent<StickToPlanet>();
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

        _pRigidbody.MovePosition(_pRigidbody.transform.position + _movement);
    }

    void Turn(float h, float v)
    {
        // smooth rotation with damping parameter
        if (_isWalking)
        {
            Transform mainCamera = Camera.main.transform;

            _turnDirection = Quaternion.LookRotation(mainCamera.up, _stickToPlanet.PlanetCurrentNormal) * new Vector3(h, 0f, v);
            Quaternion rotAround = Quaternion.LookRotation(_turnDirection, _stickToPlanet.PlanetCurrentNormal);
            // look forward to input plane direction
            _pRigidbody.MoveRotation(Quaternion.Slerp(_pRigidbody.transform.rotation, rotAround, Damping * Time.deltaTime));
        }
        else
        {
            Quaternion rotAround = Quaternion.FromToRotation(transform.up, _stickToPlanet.PlanetCurrentNormal);            
            // rot to up vector based on gravity
            _pRigidbody.MoveRotation(Quaternion.Slerp(_pRigidbody.transform.rotation, rotAround * _pRigidbody.transform.rotation, Damping * Time.deltaTime));
            // kill angular rotation from accumulated force on rigidbody
            // _pRigidbody.angularVelocity *= 1f - 1f / Damping;
            _pRigidbody.AddTorque(-_pRigidbody.angularVelocity * KillAngularVelocity);
        }
    }

    void Jump(float f)
    {
        if (f <= 0f) return;

        _pRigidbody.velocity = transform.up * f * JumpStrength;
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            // velocity vector direction
            Gizmos.color = Color.magenta;
            DrawArrow.ForGizmo(transform.position, _pRigidbody.velocity);

            // axis input plane
            Gizmos.color = Color.red;
            DebugExtension.DrawCircle(transform.position, _stickToPlanet.PlanetCurrentNormal, Color.red, 1f);
        }

        // draw initial velocity vector
        float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

        // draw current input position
        Gizmos.DrawCube(transform.position + _turnDirection, Vector3.one * 0.15f);
    }
}
