using System;
using UnityEngine;

[RequireComponent(typeof (ActorStatus))]
public class StickToPlanet : MonoBehaviour
{
    [Flags]
    public enum StickStatus
    {
        Stranded = (1 << 0), // no link close
        OnGround = (1 << 1), // close to planetary body
        Flying = (1 << 2), // linked but not close to planet
        ChangingPlanet = (1 << 3) // moving to another planetary body
    }

    private ActorStatus _actorStatus;

    private Vector3 _downDirection;
    private RaycastHit _hitDown, _hitUp;
    private Transform _onPlanet;
    private int _planetLayer; // all planets reside in this layer
    private Vector3 _targetNormal;
    public float GroundMinimumDistance = 0.65f;
    public float LinkToPlanetDistance = 10f;
    public float PlanetChangeDamping = 3f;
    [BitMask(typeof (StickStatus))] public StickStatus Status = StickStatus.Stranded;
    public Vector3 PlanetCurrentNormal { get; private set; }

    private void Awake()
    {
        _planetLayer = LayerMask.GetMask("Planets");
        _actorStatus = GetComponent<ActorStatus>();
    }

    private void LateUpdate()
    {
        _downDirection = transform.TransformDirection(Vector3.down);

        var notStranded = false;
        // link ray from transform to ground
        if (Physics.Raycast(transform.position, _downDirection, out _hitDown, LinkToPlanetDistance, _planetLayer))
        {
            notStranded = true;
            LinkToPlanet(_hitDown);
        }

        // link ray from transform to whatever is on it
        if (Physics.Raycast(transform.position, transform.up, out _hitUp, LinkToPlanetDistance, _planetLayer))
        {
            if ((Status & StickStatus.ChangingPlanet) == 0 &&
                (Status & (StickStatus.Flying | StickStatus.Stranded)) > 0 &&
                _hitUp.distance < _hitDown.distance)
            {
                notStranded = true;
                // LinkToPlanet(_hitUp);
            }
        }

        Status = notStranded ? Status : StickStatus.Stranded;
    }

    private void LinkToPlanet(RaycastHit hit)
    {
        var lastPlanet = _onPlanet;
        _onPlanet = hit.transform;
        // change status based on distance to ground
        Status = hit.distance < GroundMinimumDistance
            ? StickStatus.OnGround
            : (Status | StickStatus.Flying) & ~StickStatus.OnGround;

        // change of active planet
        Status |= _onPlanet != lastPlanet && hit.distance >= GroundMinimumDistance
            ? StickStatus.ChangingPlanet
            : Status;

        if ((_actorStatus.MoveDirection & ActorStatus.MovingTo.Down) > 0)
        {
            Status = Status & ~StickStatus.ChangingPlanet;
        }

        if ((Status & StickStatus.ChangingPlanet) > 0)
        {
            _targetNormal = -hit.normal;
        }
        // change normal with closest planet body hit
        if(_onPlanet == lastPlanet) PlanetCurrentNormal = hit.normal;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position, transform.position + _downDirection * LinkToPlanetDistance);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(transform.position, transform.position + transform.up * LinkToPlanetDistance);
        // stick planet
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, _hitDown.point);
        Gizmos.color = Color.red;
        DrawArrow.ForGizmo(transform.position + _targetNormal, _targetNormal);
    }
}