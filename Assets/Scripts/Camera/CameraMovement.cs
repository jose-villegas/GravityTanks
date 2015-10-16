using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {
    public Transform Target;
    public float HeightToTarget = 10f;
    public float Smoothing = 5f;

    public Transform NonInterpolatedTransform { get; private set; }

    void Start ()
    {
        GameObject go = new GameObject("NonInterpolatedTransform");
        NonInterpolatedTransform = go.transform;
        go.transform.SetParent(transform, false);
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 targetCamPos = Target.position + Target.up * HeightToTarget;
        Vector3 targetDirection = Target.position - transform.position;
        transform.position = Vector3.Slerp(transform.position, targetCamPos, Smoothing * Time.deltaTime);

        Quaternion lookToPlayer = Quaternion.LookRotation(targetDirection, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookToPlayer, Smoothing * Time.deltaTime);

	    NonInterpolatedTransform.position = targetCamPos;
	    NonInterpolatedTransform.rotation = lookToPlayer;
	}

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawLine(transform.position, Target.position);
    }
}
