using System.Collections.Generic;
using System.Linq;
using GTUtils;
using UnityEngine;

namespace GTCore.Enviroment
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphericalGravity))]
    public class SatelliteMovement : MonoBehaviour
    {
        /// <summary>
        ///     Collection of precomputed orbit points
        /// </summary>
        private readonly List<Vector3> _orbitPoints = new List<Vector3>();

        private SphericalGravity _gravityPull;
        private Rigidbody _satelliteRigidbody;
        private float _timer;

        /// <summary>
        ///     Precision of the final point list, higher = less points
        /// </summary>
        [Tooltip("Precision of the final point list (higher = less points)")]
        public uint CurvePrecision = 100;

        /// <summary>
        ///     Distance * Time simulation step through orbit, higher = less
        ///     points
        /// </summary>
        [Tooltip("Distance * Time simulation step through orbit")]
        public float DistanceTimeStep = 0.02f;

        /// <summary>
        ///     Gravitational bodies which this object interacts with
        /// </summary>
        [Tooltip("Gravitational bodies which this object interacts with")]
        public List<SphericalGravity> GravityObjects;

        /// <summary>
        ///     Maximum number of points to simulate orbit, important to stop
        ///     long simulations
        /// </summary>
        [Tooltip("Maximum number of points to simulate orbit")]
        public int MaxSimulationCount = 10000;

        /// <summary>
        ///     Controls the speed in which the object complets its orbit
        /// </summary>
        [Tooltip("Controls the speed in which the object complets its orbit")]
        public float OrbitalSpeedFactor = 1f;

        /// <summary>
        ///     Distance of the last point to the first point on orbit to be
        ///     considered stable
        /// </summary>
        [Tooltip("Distance of the last point on orbit to the first")]
        public float OrbitToleranceDistance = 0.05f;

        public Vector3 StartingVelocity = Vector3.up;

        private void Awake()
        {
            _satelliteRigidbody = GetComponent<Rigidbody>();
            _gravityPull = GetComponent<SphericalGravity>();
        }

        private void Start()
        {
            ComputeTrajectory();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private Vector3 CurveAt(float time)
        {
            if ( time < 0f || time > 1f )
            {
                return Vector3.zero;
            }

            var index = (int)(time * (_orbitPoints.Count - 1));
            var atTime = (time * (_orbitPoints.Count - 1)) - index;
            return Vector3.Slerp(_orbitPoints[index],
                _orbitPoints[Mathf.Min(index + 1, _orbitPoints.Count - 1)],
                atTime);
        }

        private void Move()
        {
            _timer += Time.fixedDeltaTime * OrbitalSpeedFactor;
            _timer %= 1f; // mod 1, values go from 0 to 1

            _satelliteRigidbody.MovePosition(
                Vector3.Slerp(_satelliteRigidbody.position, CurveAt(_timer),
                    Time.deltaTime));
        }

        private float ClosestPuller()
        {
            return
                GravityObjects.Select(
                    go =>
                        Vector3.Distance(transform.position,
                            go.transform.position))
                    .Concat(new[] { Mathf.Infinity })
                    .Min();
        }

        private float OrbitalSpeed()
        {
            return
                Mathf.Sqrt((_gravityPull.PlanetInformation.Gravity *
                            _satelliteRigidbody.mass) / ClosestPuller());
        }

        private void ComputeTrajectory()
        {
            float angle = 0;
            var dt = DistanceTimeStep;

            var orbitPoint = transform.position;
            var lastOrbitPoint = orbitPoint;

            var velocity = StartingVelocity;

            float temporalAngleSum = 0;
            var step = 0;
            _orbitPoints.Clear();

            while ( angle < 360 && step < MaxSimulationCount )
            {
                if ( step % CurvePrecision == 0 )
                {
                    _orbitPoints.Add(orbitPoint);
                    angle += temporalAngleSum;
                    temporalAngleSum = 0;

                    var distanceT = Vector3.Distance(orbitPoint, _orbitPoints[0]);
                    if ( distanceT < OrbitToleranceDistance &&
                         _orbitPoints.Count > 1 )
                    {
                        break;
                    }
                }

                var a = AccelerationCalc(GravityObjects, orbitPoint);
                velocity += a * dt;
                orbitPoint += velocity * dt;

                if ( GravityObjects.Count == 1 )
                {
                    temporalAngleSum +=
                        Mathf.Abs(Vector3.Angle(orbitPoint, lastOrbitPoint));
                }

                lastOrbitPoint = orbitPoint;
                step++;
            }

            _orbitPoints.RemoveAt(_orbitPoints.Count - 1);
        }

        private static Vector3 AccelerationCalc(
            IEnumerable<SphericalGravity> sgArray, Vector3 simPos)
        {
            var acceleration = Vector3.zero;

            foreach ( var sphericalGravity in sgArray )
            {
                var direction = sphericalGravity.transform.position - simPos;
                var gravity =
                    sphericalGravity.GetComponent<SphericalGravity>()
                        .PlanetInformation.Gravity;
                acceleration += direction.normalized * gravity /
                                direction.sqrMagnitude;
            }

            return acceleration;
        }

        private void OnDrawGizmosSelected()
        {
            // recalculates trajectorie on current parameters - cpu heavy
            if ( !Application.isPlaying )
            {
                ComputeTrajectory();
            }

            // draw initial velocity vector
            var maxScale = Mathf.Max(transform.lossyScale.x,
                transform.lossyScale.y, transform.lossyScale.z);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position,
                transform.position + StartingVelocity * maxScale);
            DrawArrow.ForGizmo(
                transform.position + StartingVelocity * maxScale,
                StartingVelocity * maxScale);

            Gizmos.color = Color.magenta;
            // draws current simulation orbit points
            for ( var i = 0; i < _orbitPoints.Count - 1; i++ )
            {
                Gizmos.DrawLine(_orbitPoints[i], _orbitPoints[i + 1]);
            }
        }
    }
}