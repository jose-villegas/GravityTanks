using UnityEngine;
using System.Collections;
using UnityEditor;

public class StickToPlanet : MonoBehaviour
{
    public float LinkToPlanetDistance = 10f;
    public Vector3 PlanetCurrentNormal { get; private set; }

    private Vector3 _downDirection;
    private int _planetLayer; // all planets reside in this layer
	// Use this for initialization
	void Awake ()
	{
	    _planetLayer = LayerMask.GetMask("Planets");
	}

    // Update is called once per frame
    void Update ()
	{
	    _downDirection = transform.TransformDirection(Vector3.down);

        RaycastHit hitDown, hitUp;
        if (Physics.Raycast(transform.position, _downDirection, out hitDown, LinkToPlanetDistance, _planetLayer))
        {
            PlanetCurrentNormal = hitDown.normal;
        }
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        DrawArrow.ForGizmo(transform.position, transform.position + PlanetCurrentNormal);
    }
}
