using UnityEngine;

public class StickToPlayer : MonoBehaviour
{
    private StickToPlanet _stickToPlanet;
    public Transform Target;

    private void Awake()
    {
        _stickToPlanet = Target.gameObject.GetComponent<StickToPlanet>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.position = Target.position;
        transform.up = _stickToPlanet.PlanetCurrentNormal;
    }
}