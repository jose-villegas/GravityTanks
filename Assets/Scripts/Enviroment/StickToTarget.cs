using UnityEngine;
using System.Collections;

public class StickToTarget : MonoBehaviour {

    public Transform target;


    PlayerMovement pMovement;

    void Awake()
    {
        pMovement = target.gameObject.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = target.position;
        transform.up = transform.position - pMovement.gravityPuller.transform.position;
    }
}
