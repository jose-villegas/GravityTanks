using System;

using GTCore.General;
using GTCore.Utils;

using UnityEngine;

namespace GTCore.Player
{
    [RequireComponent(typeof(ActorStatus))]
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

        private ActorStatus actorStatus;

        private Vector3 downDirection;
        private RaycastHit hitDown;
        private RaycastHit hitUp;
        private Transform onPlanet;
        private int planetLayer; // all planets reside in this layer
        public float GroundMinimumDistance = 0.65f;
        public float LinkToPlanetDistance = 10f;
        public float PlanetChangeDamping = 3f;

        [BitMask(typeof(StickStatus))]
        public StickStatus Status = StickStatus.Stranded;

        public Vector3 PlanetCurrentNormal { get; private set; }

        private void Awake()
        {
            planetLayer = LayerMask.GetMask("Planets");
            actorStatus = GetComponent<ActorStatus>();
        }

        private void LateUpdate()
        {
            downDirection = transform.TransformDirection(Vector3.down);

            var notStranded = false;
            // link ray from transform to ground
            if (Physics.Raycast(transform.position, downDirection, out hitDown,
                LinkToPlanetDistance, planetLayer))
            {
                notStranded = true;
                // change status based on distance to ground
                Status = hitDown.distance < GroundMinimumDistance
                    ? StickStatus.OnGround
                    : (Status | StickStatus.Flying) & ~StickStatus.OnGround;
                // normal extracted from ground hit
                PlanetCurrentNormal = hitDown.normal;
            }

            // link ray from transform to whatever is on it
            if (Physics.Raycast(transform.position, transform.up, out hitUp,
                LinkToPlanetDistance, planetLayer))
            {
                if (hitUp.distance > hitDown.distance ||
                    (Status & StickStatus.OnGround) > 0)
                {
                    return;
                }

                Debug.Log(hitUp);
            }

            Status = notStranded ? Status : StickStatus.Stranded;
        }

        private void LinkToPlanet(RaycastHit hit)
        {
            var lastPlanet = onPlanet;
            onPlanet = hit.transform;
            // change status based on distance to ground
            Status = hit.distance < GroundMinimumDistance
                ? StickStatus.OnGround
                : (Status | StickStatus.Flying) & ~StickStatus.OnGround;

            // change of active planet
            Status |= onPlanet != lastPlanet &&
                      hit.distance >= GroundMinimumDistance
                ? StickStatus.ChangingPlanet
                : Status;

            if ((actorStatus.MoveDirection & ActorStatus.MovingTo.Down) > 0)
            {
                Status = Status & ~StickStatus.ChangingPlanet;
            }

            if ((Status & StickStatus.ChangingPlanet) > 0)
            {
            }
            // change normal with closest planet body hit
            if (onPlanet == lastPlanet)
            {
                PlanetCurrentNormal = hit.normal;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            DrawArrow.ForGizmo(transform.position, hitDown.normal);
            Gizmos.color = Color.red;
            DrawArrow.ForGizmo(transform.position, hitUp.normal);
        }
    }
}