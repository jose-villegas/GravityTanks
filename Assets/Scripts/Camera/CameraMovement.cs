using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float HeightToTarget = 10f;
    public float Smoothing = 5f;
    public Transform Target;

    public Transform NonInterpolatedTransform { get; private set; }

    private void Start()
    {
        var go = new GameObject("NonInterpolatedTransform");
        NonInterpolatedTransform = go.transform;
        go.transform.SetParent(transform, false);
    }

    // Update is called once per frame
    private void Update()
    {
        var targetCamPos = Target.position + Target.up*HeightToTarget;
        var targetDirection = Target.position - transform.position;
        transform.position = Vector3.Slerp(transform.position, targetCamPos, Smoothing*Time.deltaTime);

        var lookToPlayer = Quaternion.LookRotation(targetDirection, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookToPlayer, Smoothing*Time.deltaTime);

        NonInterpolatedTransform.position = targetCamPos;
        NonInterpolatedTransform.rotation = lookToPlayer;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawLine(transform.position, Target.position);
    }
}