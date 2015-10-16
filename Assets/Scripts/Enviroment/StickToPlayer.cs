using UnityEngine;
using System.Collections;

public class StickToPlayer : MonoBehaviour
{
    public Transform Target;
    private PlayerMovement _playerMovement;

    void Awake()
    {
        _playerMovement = Target.gameObject.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Target.position;
        transform.up = transform.position - _playerMovement.GravityPuller.transform.position;
    }
}
