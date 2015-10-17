using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMovement : MonoBehaviour
{
    /// <summary>
    /// Planetary body where this object resides
    /// </summary>
    public Transform Planet;
    public Transform Player;

    public MovementBehaviour MovementType;
    public bool UseSquareDistance = false;
    public float VelocityImpulse = 5f;
    public float OutRangeSlowdown = 1f;

    public float CapsuleRadius = 1f;
    /// <summary>
    /// Traslates the area detection capsule beginning position
    /// from the object position, aligned with Planet center of mass
    /// </summary>
    public Vector3 CapsuleBegin = Vector3.zero;
    /// <summary>
    /// Translates the area detection capsule ending position
    /// from the object position, aligned with Planet center of mass
    /// </summary>
    public Vector3 CapsuleEnd = Vector3.zero;

    private Rigidbody _eRigidbody;
    private bool _isPlayerInRange;
    private float _distanceToPlayer;
    private int _playerLayer;
    
    public enum MovementBehaviour
    {
        Fixed,
        Constant,
        IncreaseOnDistance,
        DecreaseOnDistance,
    }

    void Awake()
    {
        _eRigidbody = GetComponent<Rigidbody>();
        _playerLayer = LayerMask.GetMask("PlayerLayer");
    }

    void Start()
    {
        InvokeRepeating("DetectPlayer", 0.0f, 0.65f);
    }

    void FixedUpdate()
    {
        if (_isPlayerInRange)
        {
            Vector3 forceIntensity = Vector3.zero;
            Vector3 targetDirection = (Player.position - transform.position).normalized;

            switch (MovementType)
            {
                case MovementBehaviour.Constant:
                    forceIntensity = VelocityImpulse * targetDirection; break;
                case MovementBehaviour.DecreaseOnDistance:
                    forceIntensity = VelocityImpulse * targetDirection / _distanceToPlayer; break;
                case MovementBehaviour.IncreaseOnDistance:
                    forceIntensity = VelocityImpulse * targetDirection * _distanceToPlayer; break;
            }

            // apply force on object rigidbody
            if (MovementType != MovementBehaviour.Fixed) { _eRigidbody.AddForce(forceIntensity); }
            else { _eRigidbody.velocity = targetDirection * VelocityImpulse * Time.fixedDeltaTime; }
        }
        else // slowly slow down
        {
            _eRigidbody.velocity = Vector3.Lerp(_eRigidbody.velocity, Vector3.zero, Time.fixedDeltaTime * OutRangeSlowdown);
        }
    }

    void DetectPlayer()
    {
        Vector3 targetNormal = (transform.position - Planet.position).normalized;
        Vector3 capsuleB = Quaternion.FromToRotation(Vector3.up, targetNormal) * CapsuleBegin;
        Vector3 capsuleE = Quaternion.FromToRotation(Vector3.up, targetNormal) * CapsuleEnd;

        _isPlayerInRange = Physics.CheckCapsule(transform.position + capsuleB, transform.position + capsuleE, CapsuleRadius, _playerLayer);
        _distanceToPlayer = _isPlayerInRange ?
                                UseSquareDistance ?
                                    Vector3.SqrMagnitude(transform.position - Player.position) :
                                    Vector3.Distance(transform.position, Player.position)  :
                            Mathf.Infinity;
    }

    void OnDrawGizmosSelected()
    {
        // gravity opposite direction
        if (_eRigidbody != null)
        {
            Gizmos.color = Color.cyan;
            DrawArrow.ForGizmo(transform.position, _eRigidbody.velocity);
        }

        Vector3 targetNormal = (transform.position - Planet.position).normalized;
        Vector3 capsuleB = Quaternion.FromToRotation(Vector3.up, targetNormal) * CapsuleBegin;
        Vector3 capsuleE = Quaternion.FromToRotation(Vector3.up, targetNormal) * CapsuleEnd;
        // bomb player detection range
        DebugExtension.DrawCapsule(
            transform.position + capsuleB,
            transform.position + capsuleE,
            _isPlayerInRange ? Color.red : Color.gray,
            CapsuleRadius
        );

        if (_isPlayerInRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                transform.position,
                transform.position + 
                (Player.position - transform.position).normalized 
                * _distanceToPlayer
            );
        }
    }
}
