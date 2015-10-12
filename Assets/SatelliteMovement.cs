using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// on rails accurate orbit
public class SatelliteMovement : MonoBehaviour {

    public List<SphericalGravity> gravityObjects;
    public Vector3 startingVelocity = Vector3.up;

    // precision of the final point list, higher = less points
    [Tooltip("Precision of the final point list (higher = less points)")]
    public int precision = 100;
    // maximum number of points to simulate orbit, important to stop long simulations
    [Tooltip("Maximum number of points to simulate orbit, stops long simulations")]
    public int maxSimulationCount = 10000;
    // distance * time simulation step through orbit
    [Tooltip("Distance * Time simulation step through orbit")]
    public float distanceTime = 0.05f;

    public bool recalculate = false;

    public float orbitalSpeed;
    public float orbitalSpeedFactor = 1f;

    public SphericalGravity gPullRange;

    Rigidbody sRigidbody;
    Transform parentTransform;

    List<Vector3> OrbitPoints = new List<Vector3>();

    int pullersMask;
    float timer = 0f;

    void Start()
    {
        sRigidbody = GetComponent<Rigidbody>();
        pullersMask = LayerMask.GetMask("GPullRange");
        ComputeTrajectory();

        parentTransform = transform.parent.transform;
    }

    void Update()
    {
        // recalculates trajectorie on current parameters - cpu heavy
        if (recalculate == true)
        {
            recalculate = !recalculate;
            ComputeTrajectory();
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    Vector3 CurveAt(float t)
    {
        if (t < 0f || t > 1f) return Vector3.zero;

        int i = (int)(t * (OrbitPoints.Count - 1));
        float f = (t * (OrbitPoints.Count - 1)) - i;
        return Vector3.Slerp(OrbitPoints[i], OrbitPoints[Mathf.Min(i + 1, OrbitPoints.Count - 1)], f);
    }

    void Move()
    {
        orbitalSpeed = Mathf.Sqrt((gPullRange.gravitationalPull * sRigidbody.mass) / ClosestPuller()) * orbitalSpeedFactor; // non-accurate
        timer += Time.deltaTime * orbitalSpeed; timer %= 1f; // mod 1, values go from 0 to 1

        parentTransform.position = CurveAt(timer);
    }

    float ClosestPuller()
    {
        float distance = Mathf.Infinity;

        foreach(SphericalGravity go in gravityObjects)
        {
            float gameO = Vector3.Distance(transform.position, go.transform.position);
            if (gameO < distance) { distance = gameO; }
        }

        return distance;
    }

    void ComputeTrajectory()
    {
        float angle = 0;
        float dt = distanceTime;

        Vector3 s = transform.position;
        Vector3 lastS = s;

        Vector3 v = startingVelocity;
        Vector3 a = AccelerationCalc(gravityObjects, s);

        float tempAngleSum = 0;
        int step = 0;
        OrbitPoints.Clear();

        while (angle < 360 && step < maxSimulationCount)
        {
            if (step % precision == 0)
            {
                OrbitPoints.Add(s);
                angle += tempAngleSum;
                tempAngleSum = 0;
            }

            a = AccelerationCalc(gravityObjects, s);
            v += a * dt;
            s += v * dt;

            if (gravityObjects.Count == 1)
            {
                tempAngleSum += Mathf.Abs(Vector3.Angle(s, lastS));
            }

            lastS = s;
            step++;
        }

        OrbitPoints.RemoveAt(OrbitPoints.Count - 1);
    }

    Vector3 AccelerationCalc(List<SphericalGravity> goArray, Vector3 simPos)
    {
        Vector3 a = Vector3.zero;
        Vector3 dir;

        for (int i = 0; i < goArray.Count; i++)
        {
            dir = goArray[i].transform.position - simPos;
            float gravity = goArray[i].GetComponent<SphericalGravity>().gravitationalPull;
            a += dir.normalized * gravity / dir.sqrMagnitude;
        }

        return a;
    }

    void OnDrawGizmosSelected()
    {
        // recalculates trajectorie on current parameters - cpu heavy
        if (!Application.isPlaying)  ComputeTrajectory();
        
        // draws current simulation orbit points
        for (int i = 0; i < OrbitPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(OrbitPoints[i], OrbitPoints[i + 1]);
        }
    }
}
