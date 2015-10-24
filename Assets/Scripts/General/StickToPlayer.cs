using UnityEngine;

public class StickToPlayer : MonoBehaviour
{
    private StickToPlanet stickToPlanet;
    public Transform Target;

    private void Awake()
    {
        stickToPlanet = Target.gameObject.GetComponent<StickToPlanet>();
        // disable hieracy transform, use local transform
        transform.SetParent(transform.parent.transform, false);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.position = Target.position;
        transform.up = stickToPlanet.PlanetCurrentNormal;
    }
}