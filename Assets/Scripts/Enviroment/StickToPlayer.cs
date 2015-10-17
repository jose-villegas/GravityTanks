using UnityEngine;
using System.Collections;

public class StickToPlayer : MonoBehaviour
{
    public Transform Target;

    private StickToPlanet _stickToPlanet;
    void Awake()
    {
        _stickToPlanet = Target.gameObject.GetComponent<StickToPlanet>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Target.position;
        transform.up = _stickToPlanet.PlanetCurrentNormal;
    }
}
