using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// on rails accurate orbit
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphericalGravity))]
public class SatelliteMovement : MonoBehaviour {

    public List<SphericalGravity> GravityObjects;
    public Vector3 StartingVelocity = Vector3.up;

    // precision of the final point list, higher = less points
    [Tooltip("Precision of the final point list (higher = less points)")]
    public uint CurvePrecision = 100;
    // maximum number of points to simulate orbit, important to stop long simulations
    [Tooltip("Maximum number of points to simulate orbit, stops long simulations")]
    public int MaxSimulationCount = 10000;
    // distance * time simulation step through orbit
    [Tooltip("Distance * Time simulation step through orbit")]
    public float DistanceTime = 0.05f;

    public float OrbitalSpeedFactor = 1f;
    // distance of the last point on orbit to the first to be considered stable
    [Tooltip("Distance of the last point on orbit to the first to be considered stable")]
    public float OrbitToleranceDistance = 0.05f;

    private Rigidbody _sRigidbody;
    private SphericalGravity _gravityPull;
    private readonly List<Vector3> _orbitPoints = new List<Vector3>();
    private float _timer = 0f;

    void Awake()
    {
        _sRigidbody = GetComponent<Rigidbody>();
        _gravityPull = GetComponent<SphericalGravity>();
    }

    void Start()
    {
        ComputeTrajectory();
    }

    void FixedUpdate()
    {
        Move();
    }

    Vector3 CurveAt(float t)
    {
        if (t < 0f || t > 1f) return Vector3.zero;

        int i = (int)(t * (_orbitPoints.Count - 1));
        float f = (t * (_orbitPoints.Count - 1)) - i;
        return Vector3.Slerp(_orbitPoints[i], _orbitPoints[Mathf.Min(i + 1, _orbitPoints.Count - 1)], f);
    }

    void Move()
    {
        _timer += Time.fixedDeltaTime * OrbitalSpeedFactor; _timer %= 1f; // mod 1, values go from 0 to 1

        _sRigidbody.MovePosition(CurveAt(_timer));
    }

    float ClosestPuller()
    {
        return GravityObjects.Select(go => Vector3.Distance(transform.position, go.transform.position)).Concat(new[] {Mathf.Infinity}).Min();
    }

    float OrbitalSpeed()
    {
        return Mathf.Sqrt((_gravityPull.GravitationalPull * _sRigidbody.mass) / ClosestPuller());
    }

    void ComputeTrajectory()
    {
        float angle = 0;
        float dt = DistanceTime;

        Vector3 s = transform.position;
        Vector3 lastS = s;

        Vector3 v = StartingVelocity;
        Vector3 a = AccelerationCalc(GravityObjects, s);

        float tempAngleSum = 0;
        int step = 0;
        _orbitPoints.Clear();

        while (angle < 360 && step < MaxSimulationCount)
        {
            if (step % CurvePrecision == 0)
            {
                _orbitPoints.Add(s);
                angle += tempAngleSum;
                tempAngleSum = 0;

                float distanceT = Vector3.Distance(s, _orbitPoints[0]);
                if (distanceT < OrbitToleranceDistance && _orbitPoints.Count > 1) break;
            }

            a = AccelerationCalc(GravityObjects, s);
            v += a * dt;
            s += v * dt;

            if (GravityObjects.Count == 1)
            {
                tempAngleSum += Mathf.Abs(Vector3.Angle(s, lastS));
            }

            lastS = s;
            step++;
        }

        _orbitPoints.RemoveAt(_orbitPoints.Count - 1);
    }

    static Vector3 AccelerationCalc(IEnumerable<SphericalGravity> goArray, Vector3 simPos)
    {
        Vector3 a = Vector3.zero;

        foreach (SphericalGravity t in goArray)
        {
            Vector3 dir = t.transform.position - simPos;
            float gravity = t.GetComponent<SphericalGravity>().GravitationalPull;
            a += dir.normalized * gravity / dir.sqrMagnitude;
        }

        return a;
    }

    void OnDrawGizmosSelected()
    {
        // recalculates trajectorie on current parameters - cpu heavy
        if (!Application.isPlaying)  ComputeTrajectory();

        // draw initial velocity vector
        float maxScale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + StartingVelocity * maxScale);
        DrawArrow.ForGizmo(transform.position + StartingVelocity * maxScale, StartingVelocity * maxScale);

        Gizmos.color = Color.magenta;
        // draws current simulation orbit points
        for (int i = 0; i < _orbitPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(_orbitPoints[i], _orbitPoints[i + 1]);
        }
    }
}
