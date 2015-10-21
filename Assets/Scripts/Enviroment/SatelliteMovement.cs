using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// on rails accurate orbit
[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof (SphericalGravity))]
public class SatelliteMovement : MonoBehaviour
{
    private SphericalGravity _gravityPull;

    /// <summary>
    ///     Collection of precomputed orbit points
    /// </summary>
    private readonly List<Vector3> _orbitPoints = new List<Vector3>();

    private Rigidbody _sRigidbody;
    private float _timer;

    /// <summary>
    ///     Precision of the final point list, higher = less points
    /// </summary>
    [Tooltip("Precision of the final point list (higher = less points)")] public uint CurvePrecision = 100;

    /// <summary>
    ///     Distance * Time simulation step through orbit, higher = less points
    /// </summary>
    [Tooltip("Distance * Time simulation step through orbit")] public float DistanceTimeStep = 0.02f;

    /// <summary>
    ///     Gravitational bodies which this object interacts with
    /// </summary>
    [Tooltip("Gravitational bodies which this object interacts with")] public List<SphericalGravity> GravityObjects;

    /// <summary>
    ///     Maximum number of points to simulate orbit, important to stop long simulations
    /// </summary>
    [Tooltip("Maximum number of points to simulate orbit, stops long simulations")] public int MaxSimulationCount =
        10000;

    /// <summary>
    ///     Controls the speed in which the object complets its orbit
    /// </summary>
    [Tooltip("Controls the speed in which the object complets its orbit")] public float OrbitalSpeedFactor = 1f;

    /// <summary>
    ///     Distance of the last point to the first point on orbit to be considered stable
    /// </summary>
    [Tooltip("Distance of the last point on orbit to the first to be considered stable")] public float
        OrbitToleranceDistance = 0.05f;

    public Vector3 StartingVelocity = Vector3.up;

    private void Awake()
    {
        _sRigidbody = GetComponent<Rigidbody>();
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

    private Vector3 CurveAt(float t)
    {
        if (t < 0f || t > 1f) return Vector3.zero;

        var i = (int) (t*(_orbitPoints.Count - 1));
        var f = (t*(_orbitPoints.Count - 1)) - i;
        return Vector3.Slerp(_orbitPoints[i], _orbitPoints[Mathf.Min(i + 1, _orbitPoints.Count - 1)], f);
    }

    private void Move()
    {
        _timer += Time.fixedDeltaTime*OrbitalSpeedFactor;
        _timer %= 1f; // mod 1, values go from 0 to 1

        _sRigidbody.MovePosition(CurveAt(_timer));
    }

    private float ClosestPuller()
    {
        return
            GravityObjects.Select(go => Vector3.Distance(transform.position, go.transform.position))
                .Concat(new[] {Mathf.Infinity})
                .Min();
    }

    private float OrbitalSpeed()
    {
        return Mathf.Sqrt((_gravityPull.PlanetInformation.Gravity*_sRigidbody.mass)/ClosestPuller());
    }

    private void ComputeTrajectory()
    {
        float angle = 0;
        var dt = DistanceTimeStep;

        var s = transform.position;
        var lastS = s;

        var v = StartingVelocity;
        var a = AccelerationCalc(GravityObjects, s);

        float tempAngleSum = 0;
        var step = 0;
        _orbitPoints.Clear();

        while (angle < 360 && step < MaxSimulationCount)
        {
            if (step%CurvePrecision == 0)
            {
                _orbitPoints.Add(s);
                angle += tempAngleSum;
                tempAngleSum = 0;

                var distanceT = Vector3.Distance(s, _orbitPoints[0]);
                if (distanceT < OrbitToleranceDistance && _orbitPoints.Count > 1) break;
            }

            a = AccelerationCalc(GravityObjects, s);
            v += a*dt;
            s += v*dt;

            if (GravityObjects.Count == 1)
            {
                tempAngleSum += Mathf.Abs(Vector3.Angle(s, lastS));
            }

            lastS = s;
            step++;
        }

        _orbitPoints.RemoveAt(_orbitPoints.Count - 1);
    }

    private static Vector3 AccelerationCalc(IEnumerable<SphericalGravity> goArray, Vector3 simPos)
    {
        var a = Vector3.zero;

        foreach (var t in goArray)
        {
            var dir = t.transform.position - simPos;
            var gravity = t.GetComponent<SphericalGravity>().PlanetInformation.Gravity;
            a += dir.normalized*gravity/dir.sqrMagnitude;
        }

        return a;
    }

    private void OnDrawGizmosSelected()
    {
        // recalculates trajectorie on current parameters - cpu heavy
        if (!Application.isPlaying) ComputeTrajectory();

        // draw initial velocity vector
        var maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + StartingVelocity*maxScale);
        DrawArrow.ForGizmo(transform.position + StartingVelocity*maxScale, StartingVelocity*maxScale);

        Gizmos.color = Color.magenta;
        // draws current simulation orbit points
        for (var i = 0; i < _orbitPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(_orbitPoints[i], _orbitPoints[i + 1]);
        }
    }
}